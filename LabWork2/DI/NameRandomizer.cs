namespace LabWork2.DI
{
    public class NameRandomizer : INameRandomizer
    {
        private readonly Random m_random;

        private List<string> m_names;

        public NameRandomizer()
        {
            m_random = new Random();

            m_names = new List<string>()
        {
            "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda",
            "David", "Elizabeth", "William", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
            "Thomas", "Sarah", "Christopher", "Karen", "Charles", "Lisa", "Daniel", "Nancy",
            "Matthew", "Betty", "Anthony", "Sandra", "Mark", "Margaret", "Donald", "Ashley",
            "Steven", "Kimberly", "Andrew", "Emily", "Paul", "Donna", "Joshua", "Michelle",
            "Kenneth", "Dorothy", "Kevin", "Carol", "Brian", "Amanda", "George", "Melissa",
            "Edward", "Deborah"
        };

        }

        public string GetName()
        {
            var l = m_names.Count;

            return m_names[m_random.Next(0, l + 1)];
        }
    }
}
