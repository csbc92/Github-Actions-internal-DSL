using System.Collections.Generic;
using DSLPipeline.MetaModel.Builders.v2.Interfaces;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Steps;

namespace DSLPipeline.MetaModel.Builders.v2.Implementations
{
    public class GlobalConfigurationBuilderImpl : IGlobalConfigBuilder
    {

        private IPipelineBuilder _pipelineBuilder;

        private GlobalConfiguration _globalConfiguration;
        private IDictionary<string, string> _globalEnvironmentVars;
        private OperatingSystem _globalOs;
        private IStepsBuilder _stepsBuilder;

        public GlobalConfigurationBuilderImpl(IPipelineBuilder pipelineBuilder)
        {
            _pipelineBuilder = pipelineBuilder;
            _globalEnvironmentVars = new Dictionary<string, string>();
        }
        
        public IStepsBuilder AddStep(string name)
        {
            if (_stepsBuilder == null)
                _stepsBuilder = new StepsBuilderImpl(_pipelineBuilder);

            return _stepsBuilder.AddStep(name);
        }

        public IGlobalConfigBuilder SetEnvVar(string key, string value)
        {
            _globalEnvironmentVars.Add(key, value);
            return this;
        }

        public IGlobalConfigBuilder RunsOn(OperatingSystem os)
        {
            _globalOs = os;
            
            return this;
        }

        public IJobBuilder AddJob(string id)
        {
            return _pipelineBuilder.AddJob(id);
        }
        
        public GlobalConfiguration Collect()
        {
            return _globalConfiguration;
        }

        public void Build()
        {
            JobConfiguration globalJobConfig = BuildGlobalJobConfig();
            _globalConfiguration = new GlobalConfiguration(globalJobConfig);
            
            AddStepsToGlobalConfig();
        }

        private void AddStepsToGlobalConfig()
        {
            _stepsBuilder.Build();
            foreach (Step s in _stepsBuilder.Collect())
            {
                _globalConfiguration.AddStep(s);
            }
        }

        private JobConfiguration BuildGlobalJobConfig()
        {
            JobConfiguration globalJobConfig = new JobConfiguration(_globalOs);
            foreach (var globalEnvironmentVar in _globalEnvironmentVars)
            {
                globalJobConfig.AddEnvVar(globalEnvironmentVar.Key, globalEnvironmentVar.Value);
            }

            return globalJobConfig;
        }
    }
}