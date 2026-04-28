namespace Core.Execution
{
    public interface IExecutionContext
    {
        string GetConfigurationValue(string path);
    }
}