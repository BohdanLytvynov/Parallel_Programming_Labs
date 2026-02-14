// See https://aka.ms/new-console-template for more information
using LabWork3.Results;
using LabWork3.SearchArg;
using LabWork3.WorkQueues;
using static LabWorks.Common.Helpers.ConsoleIOHelper;

object m_sumLock = new object();

PrintCenter("Lab Work 3", ConsoleColor.Green);

Print("Creating Work Queue...", ConsoleColor.Green);

string wordForSearch = "технология";

int fileCount = 5;
int totalWordCount = 0;
string pathToExe = AppDomain.CurrentDomain.BaseDirectory;
string pathToFiles = pathToExe + Path.DirectorySeparatorChar + "Data";

IWorkingQueue<WordCountResult, int> workingQueue = new WorkingQueue<WordCountResult, int>(fileCount);

for (int i = 0; i < fileCount; i++)
{ 
    int fileId = i + 1;
    string pathToFile = pathToFiles +
        Path.DirectorySeparatorChar + $"Text{fileId}.txt";
    SearchArgs searchArgs = new SearchArgs() { PathToFile = pathToFile, Word = wordForSearch };
    workingQueue.Enqueue(Search, searchArgs, GetResult);
}

workingQueue.Dispose();//When we come here we will wait until all tasks will be done
Print($"Search Results: {totalWordCount}", ConsoleColor.Green);
Print("Program finished!", ConsoleColor.White);

void Search(object args, out WordCountResult countResult)
{
    int total = 0;
    countResult = new WordCountResult();
    SearchArgs searchArgs = args as SearchArgs;
    if (searchArgs == null)
    {
        countResult.Exception = new InvalidOperationException("Unable to cast args to the SearchArgs object!");
        return;
    }

    string pathToFile = searchArgs.PathToFile;
    if (string.IsNullOrEmpty(pathToFile))
    {
        countResult.Exception = new InvalidOperationException("Path to file was empty!");
        return;
    }

    try
    {
        using (StreamReader streamReader = new StreamReader(pathToFile))
        {
            string low = searchArgs.Word.ToLower();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                if (line == null) break;

                var arr = line.ToLower().Split(low);
                total += arr.Length - 1;
            }
        }
    }
    catch (Exception ex)
    {
        countResult.Exception = ex;
    }

    countResult.Result = total;
}

void GetResult(WordCountResult wordCountResult)
{
    if (wordCountResult == null)
        return;
    if (wordCountResult.Success)
    {
        lock (m_sumLock)
        {
            totalWordCount += wordCountResult.Result;
        }
        Print($"Thread: {wordCountResult.Name} finished. Result: {wordCountResult.Result} ", ConsoleColor.Yellow);
    }
    else
    {
        Print($"Thread: {wordCountResult.Name} finished with Error! \n\tError: {wordCountResult.Exception.Message}",
            ConsoleColor.Red);
    }
}
