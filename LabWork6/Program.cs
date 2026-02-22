// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using static LabWorks.Common.Helpers.ConsoleIOHelper;

PrintCenter("Lab Work 6", ConsoleColor.Green);

//dy/dx = 4x y = 2x^2

//Solution of the ODE
static double CheckFunc(double x)
{
    return 2 * x * x;
}

static double DerivFunc(double x)
{
    return 4 * x;
}

Stopwatch stopwatch = null;
object printLock = new object();
do
{
    Console.Clear();

    //Object to handle cancel request
    CancellationTokenSource tokenSource = new CancellationTokenSource();
    object goToExitLock = new object();
    bool goToExit = false;
    bool cancel = false;

    double start = DoInputWhileOk<double>("Please enter the start value for solution:", ConsoleColor.Yellow, StrToDouble);
    double end = DoInputWhileOk<double>("Please enter the end value for solution:", ConsoleColor.Yellow, StrToDouble,
        (double v, out string error) =>
        {
            error = string.Empty;
            if (start < v)
            {
                return true;
            }

            error = "End value can't be less then start!";
            return false;
        });

    int n = DoInputWhileOk<int>("Please enter the amount of dx: ", ConsoleColor.Yellow, StrToInt,
        ValidateIntGreaterThenZero);

    int amount = DoInputWhileOk<int>("Please enter the amount of calculation tasks: ", ConsoleColor.Yellow,
        StrToInt, ValidateIntGreaterThenZero);

    double totalRange = end - start;
    double h = totalRange / n;

    Task<double>[] tasks = new Task<double>[amount];
    Dictionary<int, int> taskIdIndexMap = new Dictionary<int, int>();
    int stepsPerTask = n / amount;

    for (int i = 0; i < amount; i++)
    {
        int localI = i;
        tasks[i] = Task.Run(() =>
        {
            double tempSum = 0;
            int startStep = localI * stepsPerTask;
            int endStep = (localI == amount - 1) ? n : (localI + 1) * stepsPerTask;

            for (int j = startStep; j < endStep; j++)
            {
                tokenSource.Token.ThrowIfCancellationRequested();

                double x = start + j * h;
                tempSum += DerivFunc(x) * h;
            }
            return tempSum;
        });

        taskIdIndexMap.Add(tasks[i].Id, i);
    }

    stopwatch = Stopwatch.StartNew();

    var task = Task.WhenAll(tasks).ContinueWith(t =>
    {
        stopwatch.Stop();
        double calcRes = 0;
        int taskId = -1;

        try//Case when all is Ok
        {
            for (int i = 0; i < amount;  ++i)
            {
                taskId = tasks[i].Id;
                calcRes += tasks[i].Result;
            }

            lock (printLock)
            {
                Print($"Solution Found: {calcRes}", ConsoleColor.Green);
                Print("Checking solution...", ConsoleColor.Yellow);
                var realValue = CheckFunc(end) - CheckFunc(start);
                Print($"Real value: {realValue}", ConsoleColor.Green);
            }
        }
        catch (AggregateException ex)//Some Exception happened, Handling cancelation
        {
            ex.Flatten().Handle(x =>
            {
                if (x is OperationCanceledException fex)
                {
                    lock (printLock)
                    {
                        Print($"Calculation for index {GetIndex(taskId, taskIdIndexMap)} was canceled!", ConsoleColor.Cyan);
                    }

                    return true;
                }

                return false;
            });
        }
        catch (Exception ex)//Handling other Errors
        {
            lock (printLock)
            {
                Print($"Runtime error occurred during calculation of {GetIndex(taskId, taskIdIndexMap)}." +
                    $" \nError:{ex.Message}",
                    ConsoleColor.Red);
            }
        }
        finally//Release all the task resources. We do that cause we use App Cycle
        {
            for (int i = 0; i < amount; ++i)
            {
                tasks[i].Dispose();
            }
        }

        Print($"Total Time Elapsed: {stopwatch.Elapsed.TotalSeconds} s", ConsoleColor.Yellow);
        tokenSource.Dispose();
        lock (printLock)
        {
            goToExit = true;
        }

        if (!cancel)
        {
            Print("\nPress Any Key...", ConsoleColor.Yellow);
        }
    });

    //Handling of cancelation
    if (KeyPressed(ConsoleKey.C, "Press c to stop calculation.", ConsoleColor.Cyan))
    {
        Print("\nCancelation was requested!", ConsoleColor.Yellow);
        tokenSource.Cancel();
        cancel = true;
        try
        {
            //We block calling thread and try to wait until cancelation will be finished
            Task.WaitAll(tasks);
        }
        catch (AggregateException)//We will collect and filter OperationCanceled Exceptions
        {

        }
    }

    while (true)//Dummy cycle
    {
        lock (goToExitLock)
        {
            if (goToExit)
                break;
        }

        Thread.Sleep(TimeSpan.FromSeconds(1));
    }

} while (!KeyPressed(ConsoleKey.Escape, "\nIf you want to exit, press esc, to continue - press any key.", ConsoleColor.Yellow));

Print("Program finished!", ConsoleColor.White);

static int GetIndex(int taskId, Dictionary<int, int> taskIndexMap)
{
    int index = -1;

    taskIndexMap.TryGetValue(taskId, out index);

    return index;
}

//Converters 
static int StrToInt(string value, out bool res, out string error)
{
    res = false;
    error = string.Empty;
    int v = int.MaxValue;

    if (int.TryParse(value, out v))
    {
        res = true;
        return v;
    }

    error = $"Can't convert value <{value}> to integer!";
    return v;
}

static double StrToDouble(string value, out bool res, out string error)
{
    res = false;
    error = string.Empty;
    double v = int.MaxValue;

    if (double.TryParse(value, out v))
    {
        res = true;
        return v;
    }

    error = $"Can't convert value <{value}> to double!";
    return v;
}

//Validators
static bool ValidateIntGreaterThenZero(int value, out string error)
{
    error = string.Empty;
    if (value <= 0)
    {
        error = "Value can't be zero or less!";
        return false;
    }

    return true;
}

