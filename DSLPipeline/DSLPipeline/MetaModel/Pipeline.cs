using System;
using System.Collections.Generic;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Jobs;

namespace DSLPipeline.MetaModel
{
    public class Pipeline
    {
        public string Name { get; }

        private LinkedList<Job> _jobSequence;

        private GlobalConfiguration _globalConfiguration;
        private HashSet<TriggerType> _triggerTypes;

        public Pipeline(string name, TriggerType triggerType)
        {
            Name = name;
            _jobSequence = new LinkedList<Job>();
            _triggerTypes = new HashSet<TriggerType>();
            _triggerTypes.Add(triggerType);

        }

        /// <summary>
        /// Adds a global configuration that will be applied to all jobs in the pipeline
        /// unless overridden.
        /// </summary>
        /// <param name="globalConfig"></param>
        public void addGlobalConfiguration(GlobalConfiguration globalConfig)
        {
            _globalConfiguration = globalConfig;
        }
        
        public void addJob(Job job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            
            if (_jobSequence.Contains(job))
                throw new ArgumentException("Job already added");

            _jobSequence.AddLast(job);
        }

        public void addTrigger(TriggerType triggerType)
        {
            if (_triggerTypes.Contains(triggerType))
                throw new ArgumentException("The trigger type is already added");

            if (_triggerTypes.Count > 1)
                throw new ArgumentOutOfRangeException(nameof(triggerType), "There can only be two triggers [push, pull_request]");

            _triggerTypes.Add(triggerType);
        }
        

        /// <summary>
        /// Returns the job with the specified id.
        /// </summary>
        /// <param name="id">The id of the job</param>
        /// <returns>Return the Job or null if the job with the id does not exist</returns>
        public Job getJob(string id)
        {
            foreach (var job in _jobSequence)
            {
                if (job.ID.Equals(id))
                {
                    return job;
                }
            }

            return null; // job does not exist
        }
    }
}