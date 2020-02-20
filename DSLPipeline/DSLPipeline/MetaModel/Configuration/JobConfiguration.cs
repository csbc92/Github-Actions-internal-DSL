using System.Collections.Generic;

namespace DSLPipeline.MetaModel.Configuration
{
    public class JobConfiguration
    {
        public OperatingSystem OperatingSystem { get; }
        private Dictionary<string, string> _environmentVariables;

        public JobConfiguration(OperatingSystem os)
        {
            OperatingSystem = os;
            _environmentVariables = new Dictionary<string, string>();
        }

        
        /// <summary>
        /// Adds a new environment variable specific to this Job
        ///  Overrides environment variables that are already defined (including default ones)
        /// </summary>
        /// <param name="key">The key of the environment variable</param>
        /// <param name="value">The value of the environment variable</param>
        public void AddEnvVar(string key, string value)
        {
            if (!_environmentVariables.TryAdd(key, value)) // Adds if not exists
            {
                _environmentVariables[key] = value; // Overrides if exists
            }
        }
    }
}