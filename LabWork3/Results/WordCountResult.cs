using LabWork3.Results.Base;

namespace LabWork3.Results
{
    internal class WordCountResult : ConsumerResult<int>
    {
        public WordCountResult(int result, Exception exception) : base(result, exception)
        {
        }

        public WordCountResult() : base()
        {
            
        }
    }
}
