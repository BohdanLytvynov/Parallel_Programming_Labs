using LabWork5.MatrixCalculators;
using LabWork5.RandomMatrixFillers;
using System.Diagnostics;
using static LabWorks.Common.Helpers.ConsoleIOHelper;

RandomMatrixFiller randomMatrixFiller = new RandomMatrixFiller();
MatrixCalculator matrixCalculator = new MatrixCalculator();
CancellationTokenSource tokenSource = null;
object m_consoleLock = new object();
Stopwatch stopwatch = null;
bool calcFinished = false;
matrixCalculator.OnParallelCalculationStarted += MatrixCalculator_OnCalculationStarted;
matrixCalculator.OnParallelCalculationFinished += MatrixCalculator_OnParallelCalculationFinished;

do
{
    calcFinished = false;
    bool correctInput = true;
    tokenSource = new CancellationTokenSource();
    double[,] matA = null;
    double[,] matB = null;
    double[,] matC = null;
    Console.Clear();
    PrintCenter("Lab Work 5", ConsoleColor.Green);
    for (int i = 0; i < 2; ++i)
    {
        int rows = 0;
        int cols = 0;

        lock (m_consoleLock)
        {
            rows = DoInputWhileOk<int>($"Please enter the amount of rows of your {i + 1} Matrix: ", ConsoleColor.Yellow, StrToInt,
            ValidateIntGreaterThenZero);
            cols = DoInputWhileOk<int>($"Please enter the amount of columns of your {i + 1} Matrix: ", ConsoleColor.Yellow, StrToInt,
            ValidateIntGreaterThenZero);
        }

        if (i == 0)
        {
            matA = new double[rows, cols];
            randomMatrixFiller.Fill(matA);
        }
        else if (i == 1)
        {
            matB = new double[rows, cols];
            randomMatrixFiller.Fill(matB);
        }
    }

    Print("Matrixes prepared...", ConsoleColor.Green);
    Print("\nMatrix A: ", ConsoleColor.Cyan);
    PrintMatrix(matA);
    Print("\nMatrix B:", ConsoleColor.Cyan);
    PrintMatrix(matB);

    Print("Choose an operation, you'd like to perform: +, -, * (Use numpad...)", ConsoleColor.Yellow);
    ConsoleKeyInfo key;
    lock (m_consoleLock)
    {
        key = Console.ReadKey();
    }

    switch (key.Key)
    {
        case ConsoleKey.Add:
            Print("\nAdd operation chosen!", ConsoleColor.Green);
            if (!matrixCalculator.AddSubtractPossible(matA, matB))
            {
                Print("Unable to add matrices! Incorrect dimensions!", ConsoleColor.Red);
                correctInput = false;
            }
            else
            {
                long rows = matA.GetLongLength(0);
                long cols = matA.GetLongLength(1);

                if (rows >= 250 || cols >= 250)
                    matrixCalculator.ParallelAdd(matA, matB, tokenSource);
                else
                    matC = matrixCalculator.Add(matA, matB);
            }
            break;
        case ConsoleKey.Subtract:
            Print("\nSubtract operation chosen!", ConsoleColor.Green);
            if (!matrixCalculator.AddSubtractPossible(matA, matB))
            {
                Print("Unable to subtract matrices! Incorrect dimensions!", ConsoleColor.Red);
                correctInput = false;
            }
            else
            {
                long rows = matA.GetLongLength(0);
                long cols = matA.GetLongLength(1);
                if (rows >= 250 || cols >= 250)
                    matrixCalculator.ParallelSubtract(matA, matB, tokenSource);
                else
                    matC = matrixCalculator.Subtract(matA, matB);
            }
            break;
        case ConsoleKey.Multiply:
            Print("\nMultiplication operation chosen!", ConsoleColor.Green);
            if (!matrixCalculator.MultiplicationPossible(matA, matB))
            {
                Print("Unable to multiply matrices! Incorrect dimensions!", ConsoleColor.Red);
                correctInput = false;
            }
            else
            {
                long rows = matA.GetLongLength(0);
                long cols = matA.GetLongLength(1);
                if (rows >= 64 || cols >= 64)
                    matrixCalculator.ParallelSubtract(matA, matB, tokenSource);
                else
                    matC = matrixCalculator.Multiply(matA, matB);
            }
            break;
        default:
            Print("Incorrect operation!", ConsoleColor.Red);
            break;
    }

    if (!matrixCalculator.Parallelism && correctInput)
    {
        Print("Sync calculation was used.", ConsoleColor.Yellow);
        Print("Printing results:", ConsoleColor.Yellow);
        PrintMatrix(matC);
    }

} while (!KeyPressed(ConsoleKey.Escape, "\nIf you want to exit, press esc, to continue - press any key.", ConsoleColor.Yellow));

static void PrintMatrix(double[,] mat)
{
    if (mat == null) throw new ArgumentNullException(nameof(mat));

    long row = mat.GetLongLength(0);
    long col = mat.GetLongLength(1);
    object consoleLock = new object();
    if (row > 50 || col > 50)
    {
        Print("To big matrix for print! Do you want to print? [Y/N]", ConsoleColor.Yellow);
        ConsoleKey key;
        lock (consoleLock)
        {
            key = Console.ReadKey().Key;
        }

        if (key == ConsoleKey.N)
            return;
    }

    for (long i = 0; i < row; ++i)
    {
        for (long j = 0; j < col; ++j)
        {
            Console.Write(mat[i, j]);
            Console.Write(" ");
        }
        Console.WriteLine();
    }
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


//Validators
static bool ValidateIntGreaterThenZero(int value, out string error)
{
    error = string.Empty;
    if (value <= 0)
    {
        error = $"Value can't be zero or less! Current Value was: <{value}>";
        return false;
    }

    return true;
}

void MatrixCalculator_OnParallelCalculationFinished(string obj, double[,] mat)
{
    stopwatch.Stop();
    Print($"Parallel {obj} of matrices Finished... Elapsed time: {stopwatch.Elapsed.Seconds} s",
        ConsoleColor.Green);
    calcFinished = true;
    PrintMatrix(mat);
}

void MatrixCalculator_OnCalculationStarted(string operation)
{
    stopwatch = Stopwatch.StartNew();

    bool cancel = false;
    Print($"Calculation of matrix {operation} started. Please wait.", ConsoleColor.Green);
    Print("Press C to stop calculation.", ConsoleColor.Yellow);
    while (!calcFinished)
    {
        ConsoleKeyInfo key;
        lock (m_consoleLock)
        {
            key = Console.ReadKey();
        }
        if (key.Key == ConsoleKey.C)
        {
            tokenSource?.Cancel();
            lock (m_consoleLock)
            {
                Print($"Cancellation of matrix {operation} requested...", ConsoleColor.Cyan);
                cancel = true;
            }
        }
    }

    if (cancel)
    {
        Print($"{operation} was cancelled!", ConsoleColor.Green);
    }
}
