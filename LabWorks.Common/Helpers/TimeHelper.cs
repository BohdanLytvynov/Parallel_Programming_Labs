namespace LabWorks.Common.Helpers
{
    public static class TimeHelper
    {
        public static double ToSeconds(long ms)
        { 
            return (double)ms / (double)1000;
        }
    }
}
