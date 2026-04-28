using Microsoft.Extensions.Configuration;

namespace Service.Execution
{
    public class ExecutionContext : IExecutionContext
    {
        private readonly IConfiguration _config;

        public ExecutionContext(IConfiguration config)
        {
            _config = config;
        }

        public string GetConfigurationValue(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException($"{nameof(path)} cannot be null.");
            }

            var value = _config[path];

            if (value is null)
            {
                throw new InvalidOperationException($"{nameof(path)} appsetting string does not exist.");
            }

            return value;
        }
    }
}
