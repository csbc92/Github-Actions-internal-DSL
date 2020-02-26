using System;
using System.Collections.Generic;
using System.Linq;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Jobs;

namespace DSLPipeline.MetaModel
{
    public class Pipeline
    {
        public string Name { get; }

        private IList<Job> _jobSequence;

        private GlobalConfiguration _globalConfiguration;
        private HashSet<TriggerType> _triggerTypes;

        public Pipeline(string name, TriggerType triggerType)
        {
            Name = name;
            _jobSequence = new List<Job>();
            _triggerTypes = new HashSet<TriggerType>();
            
            if (triggerType != null)
                _triggerTypes.Add(triggerType);

        }

        /// <summary>
        /// Adds a global configuration that will be applied to all jobs in the pipeline
        /// unless overridden.
        /// </summary>
        /// <param name="globalConfig"></param>
        public void AddGlobalConfiguration(GlobalConfiguration globalConfig)
        {
            _globalConfiguration = globalConfig;
        }
        
        public void AddJob(Job job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            
            if (_jobSequence.Contains(job))
                throw new ArgumentException("Job already added");

            _jobSequence.Add(job);
        }

        public void AddTrigger(TriggerType triggerType)
        {
            if (_triggerTypes.Contains(triggerType))
                throw new ArgumentException("The trigger type is already added");

            if (_triggerTypes.Count > 1)
                throw new ArgumentOutOfRangeException(nameof(triggerType), "There can only be two triggers [push, pull_request]");

            _triggerTypes.Add(triggerType);
        }

        public HashSet<TriggerType> TriggerTypes => new HashSet<TriggerType>(_triggerTypes);
        
        public IList<Job> Jobs => new List<Job>(_jobSequence);

        public GlobalConfiguration GlobalConfiguration => _globalConfiguration;


        /// <summary>
        /// Returns the job with the specified id.
        /// </summary>
        /// <param name="id">The id of the job</param>
        /// <returns>Return the Job or null if the job with the id does not exist</returns>
        public Job GetJob(string id)
        {
            foreach (var job in _jobSequence)
            {
                if (job.Id.Equals(id))
                {
                    return job;
                }
            }

            return null; // job does not exist
        }
    }
}