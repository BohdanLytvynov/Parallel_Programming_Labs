using System.Diagnostics;
using System.Numerics;
using static LabWorks.Common.Helpers.ConsoleIOHelper;

//Main app cycle
do
{
    Console.Clear();//Clear the Console Window

    PrintCenter("Lab Work 4", ConsoleColor.Green);
    //Object to handle cancel request
    CancellationTokenSource tokenSource = new CancellationTokenSource();
    object goToExitLock = new object();
    bool goToExit = false;
    bool cancel = false;
    object printLock = new object();//Lock for access to the console
    //Get the amount of Fibonacci numbers for calculations
    int amount = DoInputWhileOk<int>("Please enter the amount of Fibonacci numbers you'd like to calculate:", ConsoleColor.Yellow, StrToInt,
        ValidateIntGreaterThenZero);
    //Structure for mapping Task Id and the index of the Fib number
    Dictionary<int, int> taskIdIndexMap = new Dictionary<int, int>();

    Task<BigInteger>[] tasks = new Task<BigInteger>[amount];
    for (int i = 0; i < amount; i++)
    {
        //Asking user for the index of the Fib number
        int index = DoInputWhileOk<int>($"Please enter the {i + 1} index of the Fibonacci number:", ConsoleColor.Yellow, StrToInt,
        ValidateIntGreaterThenZero);

        tasks[i] = new Task<BigInteger>(() =>
        {
            Stopwatch sw = Stopwatch.StartNew();
            lock (printLock)
            {
                Print($"Starting calculation for Fibonacci number with index: {index}",
                    ConsoleColor.Yellow);
            }
            var r = Fib(index, tokenSource.Token);
            sw.Stop();
            lock (printLock)
            {
                Print($"Calculation for Fibonacci number with index: {index} finished. Time elapsed: {sw.Elapsed.Seconds} s",
                    ConsoleColor.Green);
            }

            return r;
        });

        taskIdIndexMap.Add(tasks[i].Id, index);
    }

    Print("Setting tasks...", ConsoleColor.Green);

    foreach (var kv in taskIdIndexMap)
    {
        Print($"\nTask with id: {kv.Key} will calculate Fib number with index: {kv.Value}", ConsoleColor.Cyan);
    }

    Print("\nYou can Interrupt calculations by pressing c.\n To Start calculation press any key!", ConsoleColor.Yellow);
    Console.ReadKey();

    Print("\nStarting calculation Tasks...", ConsoleColor.Green);

    Stopwatch stopwatch = Stopwatch.StartNew();

    for (int i = 0; i < amount; i++)//Start all tasks
    {
        tasks[i].Start();
    }
    //Configure continuation... After all task will be started the control will return back to the calling thread. (Main UI Thread)
    var t = Task.WhenAll(tasks)
    .ContinueWith(t =>
    {
        stopwatch.Stop();
        BigInteger calcRes = 0;
        int taskId = -1;
        for (int i = 0; i < amount; i++)
        {
            try//Case when all is Ok
            {
                taskId = tasks[i].Id;
                calcRes = tasks[i].Result;
                lock (printLock)
                {
                    Print($"Fibonacci index {GetIndex(taskId, taskIdIndexMap)} -> {calcRes}", ConsoleColor.Green);
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
            if(goToExit)
                break;
        }

        Thread.Sleep(TimeSpan.FromSeconds(1));
    }

} while (!KeyPressed(ConsoleKey.Escape, "\nIf you want to exit, press esc, to continue - press any key.", ConsoleColor.Yellow));

Print("Program finished!", ConsoleColor.White);

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

static BigInteger Fib(int n, CancellationToken cancellationToken)
{ 
    if(n <= 1) return n;

    if (cancellationToken.IsCancellationRequested)
       cancellationToken.ThrowIfCancellationRequested();

    return Fib(n - 1, cancellationToken) + Fib(n - 2, cancellationToken);
}

static int GetIndex(int taskId, Dictionary<int, int> taskIndexMap)
{
    int index = -1;

    taskIndexMap.TryGetValue(taskId, out index);

    return index;
}
