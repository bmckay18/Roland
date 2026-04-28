using Microsoft.Extensions.Configuration;

namespace Core.Execution
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
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException((nameof(path)));
            }

            var value = _config[path];

            if (value is null)
            {
                throw new KeyNotFoundException($"Missing configuration value: {path}");
            }

            return value;
        }
    }
}
