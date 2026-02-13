using static LabWorks.Common.Helpers.ConsoleIOHelper;

namespace LabWork2.Base
{
    internal abstract class FoodSupplyBase : IFoodSupply
    {
        public int FoodQuantity { get; set; }
        public string Name { get; init; }

        private readonly object m_foodSupplyLock = new object();

        private readonly List<Task> m_EatingTasks = new List<Task>();

        public void AddToFeeding(Task task)
        { 
            m_EatingTasks.Add(task);
        }

        public bool Feed<THungryEntity>(THungryEntity hungryEntity) 
            where THungryEntity : IHungryEntity
        {
            if (FoodQuantity - hungryEntity.RequiredFoodQuantity <= 0)
                return false;

            lock (m_foodSupplyLock)
            {
                FoodQuantity -= hungryEntity.RequiredFoodQuantity;
            }

            Print($"Food left: {FoodQuantity}", ConsoleColor.Red);
            return true;
        }

        public Task ConfigureWait()
        {
            if(m_EatingTasks.Count == 0)
                return Task.CompletedTask;
            //Use of non blocking 
            return Task.WhenAll(m_EatingTasks);
        }
    }
}
