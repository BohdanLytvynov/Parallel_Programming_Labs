using LabWork3.Results.Base;

namespace LabWork3.Results
{
    internal class WordCountResult : ConsumerResult<int>
    {
        public string FileName { get; set; }

        public WordCountResult(int result, Exception exception) : base(result, exception)
        {
        }

        public WordCountResult() : base()
        {
            
        }
    }
}
