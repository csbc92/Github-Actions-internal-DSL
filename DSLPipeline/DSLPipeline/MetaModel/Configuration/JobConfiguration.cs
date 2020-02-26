using System.Collections.Generic;

namespace DSLPipeline.MetaModel.Configuration
{
    public class JobConfiguration
    {
        public OperatingSystem OperatingSystem { get; set; }
        private IDictionary<string, string> _environmentVariables;

        public JobConfiguration(OperatingSystem os)
        {
            OperatingSystem = os;
            _environmentVariables = new Dictionary<string, string>();
        }

        /// <summary>
        /// Adds a new environment variable specific to this Job
        /// Does not override environment variables that are already defined
        /// </summary>
        /// <param name="key">The key of the environment variable</param>
        /// <param name="value">The value of the environment variable</param>
        public void AddEnvVar(string key, string value, bool replace = false)
        {
            if (!_environmentVariables.TryAdd(key, value)) // Adds if not exists
            {
                if (replace)
                {
                    _environmentVariables[key] = value; // Overrides if exists
                }
            }
        }

        public IDictionary<string, string> EnvironmentVariables
        {
            get
            {
                return _environmentVariables;
            }
        }
    }
}