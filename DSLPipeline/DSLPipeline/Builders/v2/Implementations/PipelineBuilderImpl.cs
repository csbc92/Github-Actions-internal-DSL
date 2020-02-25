using System;
using System.Collections.Generic;
using DSLPipeline.MetaModel;
using DSLPipeline.Builders.v2.Interfaces;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Jobs;

namespace DSLPipeline.Builders.v2.Implementations
{
    public class PipelineBuilderImpl : IPipelineBuilder
    {
        private Pipeline _pipeline = null;
        private JobConfiguration _globalJobConfiguration;
        
        private IGlobalConfigBuilder _globalConfigBuilder;
        private IJobBuilder _jobBuilder;


        // Jobs
        private IDictionary<string, Job> _jobsContext; // key is jobID

        public PipelineBuilderImpl()
        {
            // Context variables
            _jobsContext = new Dictionary<string, Job>();

            // Initialize builders
            _globalConfigBuilder = new GlobalConfigurationBuilderImpl(this);
        }
        
        public IPipelineBuilder Pipeline(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            
            _pipeline = new Pipeline(name, null);

            return this;
        }
        
        public IPipelineBuilder TriggerOn(TriggerType triggerType)
        {
            _pipeline.AddTrigger(triggerType);

            return this;
        }

        public IGlobalConfigBuilder AddGlobals()
        {
            return _globalConfigBuilder;
        }

        public Job Lookup(string jobId)
        {
            Job value = null;
            
            if (_jobsContext.TryGetValue(jobId, out value))
            {
                return value;
            }

            throw new Exception("The Job with id " + jobId + " does not exist");
        }

        public IJobBuilder AddJob(string id)
        {
            // Are we currently building a job?
            if (_jobBuilder != null)
            {
                // Add the current job to the jobs context
                BuildCurrentJob();
            }

            // Clear the existing job builder and get ready to make a new job
            _jobBuilder = new JobBuilderImpl(this);
            _jobBuilder.SetId(id);
            return _jobBuilder;
        }
        
        public void Build()
        {
            BuildCurrentJob();
            Flush();
            
            // Add jobs to the pipeline
            foreach (var keyValue in _jobsContext)
            {
                _pipeline.AddJob(keyValue.Value);
            }
            
            // Add the global configuration
            _globalConfigBuilder.Build();
            GlobalConfiguration globalConfiguration = _globalConfigBuilder.Collect();
            _pipeline.AddGlobalConfiguration(globalConfiguration);
        }

        private void BuildCurrentJob()
        {
            _jobBuilder.Build();
            Job job = _jobBuilder.Collect();
            _jobsContext.Add(job.Id, job);
        }

        private void Flush()
        {
            _jobBuilder = null;
        }

        public Pipeline Collect()
        {
            return _pipeline;
        }
    }
}