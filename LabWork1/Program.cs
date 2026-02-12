// See https://aka.ms/new-console-template for more information
using LabWork1;
using LabWorks.Common.Helpers;

ConsoleIOHelper.PrintCenter("Lab Work 1", ConsoleColor.Green);

SyncPiCalculation syncPiCalculation = new SyncPiCalculation();

long numOfSteps = 1_000_000_000;
long syncTimeElapsed = 0;
int taskCount = 1 << 4;
bool enableSync = true;
double pi = 0;

if (enableSync)
{
    ConsoleIOHelper.Print($"Starting sync calculation. Number of steps: {numOfSteps}", ConsoleColor.Green);
    pi = syncPiCalculation.Calculate(numOfSteps, out syncTimeElapsed);

    ConsoleIOHelper.Print(
        $"PI calculated: {pi}, time, elapsed for calculation: {syncTimeElapsed} ms, or {TimeHelper.ToSeconds(syncTimeElapsed)} s",
        ConsoleColor.Yellow);
}

ParallelPiCalculation parallelPiCalculation = new ParallelPiCalculation(taskCount);
long paralTimeElapse = 0;
ConsoleIOHelper.Print($"Starting parallel calculation. Number of steps: {numOfSteps}, Number of Tasks: {parallelPiCalculation.TaskCount}",
    ConsoleColor.Green);
pi = parallelPiCalculation.Calculate(numOfSteps, out paralTimeElapse);

ConsoleIOHelper.Print(
    $"PI calculated in Parallel: {pi}, time, elapsed for calculation: {paralTimeElapse} ms, or {TimeHelper.ToSeconds(paralTimeElapse)} s",
    ConsoleColor.Yellow);

double paralAcceleration = syncTimeElapsed / paralTimeElapse;

ConsoleIOHelper.Print(
    $"Parallel Acceleration: {paralAcceleration}",
    ConsoleColor.Cyan
    );

ConsoleIOHelper.Print("Program Finished...", ConsoleColor.Green);
