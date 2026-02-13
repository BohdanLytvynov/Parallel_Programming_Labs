using LabWork2.Base;
using static LabWorks.Common.Helpers.ConsoleIOHelper;

namespace LabWork2.Students
{
    internal class Student : HungryEntityBase, IHungryEntity
    {
        #region Fields
        private int m_id;
        private static int g_id;

        public string Name { get; set; }
        #endregion

        #region Ctor
        public Student() 
        {
            m_id = ++g_id;
        }
        #endregion

        #region Methods
        public void SayHello()
        {
            Print($"Hello I am {Name} and I want to eat!", ConsoleColor.Blue);
        }

        public override string ToString()
        {
            return $"{m_id}) {Name}, FQ:{RequiredFoodQuantity}, FT:{FeedingTime}";
        }

        public override void GoOutFoodSupply()
        {
            lock (m_locker)
            {
                Print($"{Name} says: ", ConsoleColor.Yellow);
            }
            base.GoOutFoodSupply();
        }
        #endregion
    }
}
