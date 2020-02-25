using System;
using System.Collections.Generic;
using System.Linq;
using DSLPipeline.Builders.v2.Interfaces;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Jobs;
using DSLPipeline.MetaModel.Steps;
using OperatingSystem = DSLPipeline.MetaModel.Configuration.OperatingSystem;

namespace DSLPipeline.Builders.v2.Implementations
{
    public class JobBuilderImpl : IJobBuilder
    {
        private OperatingSystem _runsOn;
        private IDictionary<string, string> _jobEnvironmentVariables;
        private ISet<Job> _dependsOn;
        private string _jobName;
        private string _jobId;
        private Job _currentJob;
        
        private IStepsBuilder _stepsBuilder;
        private IPipelineBuilder _pipelineBuilder;

        public JobBuilderImpl(IPipelineBuilder pipelineBuilder)
        {
            _jobEnvironmentVariables = new Dictionary<string, string>();
            _stepsBuilder = new StepsBuilderImpl(pipelineBuilder);
            _pipelineBuilder = pipelineBuilder;
            
            _dependsOn = new HashSet<Job>();
        }

        public IJobBuilder RunsOn(OperatingSystem os)
        {
            _runsOn = os;
            return this;
        }

        public IJobBuilder SetEnvVar(string key, string value)
        {
            _jobEnvironmentVariables.Add(key, value);

            return this;
        }

        public IJobBuilder DependsOn(string jobId)
        {
            Job other = _pipelineBuilder.Lookup(jobId);
            _dependsOn.Add(other);

            return this;
        }

        public IJobBuilder SetName(string jobName)
        {
            _jobName = jobName;
            return this;
        }

        public IJobBuilder SetId(string jobId)
        {
            _jobId = jobId;
            return this;
        }

        public void Build()
        {
            // Build the JobConfiguration
            JobConfiguration jobConfiguration = new JobConfiguration(_runsOn);
            foreach (var keyValue in _jobEnvironmentVariables)
            {
                jobConfiguration.AddEnvVar(keyValue.Key, keyValue.Value);
            }
            
            // Build the Steps
            _stepsBuilder.Build();
            Step[] steps = _stepsBuilder.Collect().ToArray();
            
            
            // Build the Job
            if (_jobName == null) _jobName = Guid.NewGuid().ToString();
            _currentJob = new Job(_jobId, _jobName, jobConfiguration, steps);
            // Add the job dependencies
            foreach (var job in _dependsOn)
            {
                _currentJob.AddDependency(job);
            }
        }

        public Job Collect()
        {
            return _currentJob;
        }

        public IStepsBuilder AddStep(string name)
        {
            return _stepsBuilder.AddStep(name);
        }
    }
}