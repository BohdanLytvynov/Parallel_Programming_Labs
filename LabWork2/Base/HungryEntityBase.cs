using static LabWorks.Common.Helpers.ConsoleIOHelper;
namespace LabWork2.Base
{
    internal abstract class HungryEntityBase : IHungryEntity
    {
        public int RequiredFoodQuantity { get; set; }
        public int FeedingTime { get; set; }
        public IFoodSupply FoodSupply { get; protected set; }

        protected readonly object m_locker = new object();

        public virtual void EatFood()
        {
            var t = Task.Run(() =>
            {
                try
                {
                    Print($"{ToString()} started to eat.", ConsoleColor.Green);
                    while (FoodSupply.Feed(this))
                    {
                        lock (m_locker)
                        {
                            Print($"{ToString()} is eating...", ConsoleColor.Cyan);
                        }
                        Thread.Sleep(FeedingTime);
                    }
                    Print($"{ToString()} finished eating.", ConsoleColor.Yellow);
                }
                catch (Exception ex)
                {
                }
            });

            FoodSupply.AddToFeeding(t);
        }

        public void GoToFoodSupply(IFoodSupply foodSupply)
        {
            if (foodSupply == null) throw new ArgumentNullException(nameof(foodSupply));

            FoodSupply = foodSupply;
        }

        public virtual void GoOutFoodSupply()
        {
            lock (m_locker)
            {
                Print($"There is no food in <{FoodSupply.Name}>! I am leaving it!", ConsoleColor.Red);
            }

            FoodSupply = null;
        }
    }
}
