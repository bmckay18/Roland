namespace Service.Execution
{
    public interface IExecutionContext
    {
        string GetConfigurationValue(string path);
    }
}