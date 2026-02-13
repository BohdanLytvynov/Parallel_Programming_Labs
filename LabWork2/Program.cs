// See https://aka.ms/new-console-template for more information
using LabWork2.Base;
using LabWork2.Canteens;
using LabWork2.Students;
using LabWorks.Common.FactoryWrappers.Base;
using LabWorks.Common.ObjectBuilders.Base;
using System.Reflection;
using static LabWorks.Common.Helpers.ConsoleIOHelper;

PrintCenter("Lab Work 2", ConsoleColor.Green);
object consoleLock = new object();
FactoryWrapper factoryWrapper = new FactoryWrapper(Assembly.GetCallingAssembly(), new ObjectBuilder());
IFoodSupply foodSupply = new Canteen(100, "Super Food");
int studentsCount = 2;
List<Student> students = new List<Student>();

bool stop = false;

for (int i = 0; i < studentsCount; i++)//Build Students
{ 
    var s = factoryWrapper.Create<Student>();
    s.SayHello();
    students.Add(s);
}

for (int i = 0; i < studentsCount; ++i)//Send them to food supply
{
    students[i].GoToFoodSupply(foodSupply);
}

for (int i = 0; i < studentsCount; ++i)//Tell them they can eat
{
    students[i].EatFood();
}

foodSupply.ConfigureWait()?.ContinueWith(t => 
{  
    stop = true;

    foreach (var s in students)
    {
        s.GoOutFoodSupply();
    }

});

while (!stop)//Some UI thread.
{
    lock (consoleLock)
    {
        Print("Some work of UI thread!", ConsoleColor.Magenta);
    }
    Thread.Sleep(TimeSpan.FromSeconds(0.5));
}

Print("Program finished!", ConsoleColor.White);

