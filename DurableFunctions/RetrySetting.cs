
namespace DurableFunctions
{
    public class RetrySetting
    {
        public int TimeoutInSeconds { get; set; }
        public int RetryIntervalInSeconds { get; set; }
    }
}
