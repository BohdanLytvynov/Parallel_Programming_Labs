using LabWork2.DI;
using LabWork2.Students;
using LabWorks.Common.Attributes;
using LabWorks.Common.Factories.Base;

namespace LabWork2.Factories
{
    internal class StudentFactory : AbstractFactoryBase<Student>
    {
        private readonly INameRandomizer m_randomizer;
        private readonly Random m_random;

        private readonly int m_minFoodConsumption;
        private readonly int m_maxFoodConsumption;

        private readonly int m_minFeedTime;
        private readonly int m_maxFeedTime;

        [Ctor]
        [CtorParam<INameRandomizer, NameRandomizer>]
        public StudentFactory(INameRandomizer nameRandomizer)
        {
            m_randomizer = nameRandomizer ?? throw new ArgumentNullException(nameof(nameRandomizer));
            m_random = new Random();
            ProductName = nameof(Student);
            m_maxFoodConsumption = 5;
            m_minFoodConsumption = 1;
            m_minFeedTime = 10;
            m_maxFeedTime = 100;
        }

        public override Student Create()
        {
            var student = new Student();
            student.Name = m_randomizer.GetName();
            student.RequiredFoodQuantity = m_random.Next(m_minFoodConsumption, m_maxFoodConsumption + 1);
            student.FeedingTime = m_random.Next(m_minFeedTime, m_maxFeedTime);
            return student;
        }
    }
}
