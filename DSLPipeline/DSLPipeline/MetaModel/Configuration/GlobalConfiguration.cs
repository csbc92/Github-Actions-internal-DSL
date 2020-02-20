using System.Collections.Generic;

namespace DSLPipeline.MetaModel.Configuration
{
    /// <summary>
    /// A Global Configuration is a configuration that should be applied to all Jobs.
    ///
    /// Instead of setting these manually for each Job, it is possible to set things globally such as:
    ///     Environment variables
    ///     Operating System
    ///     Steps
    ///
    /// For instance, it might be plausible to set an initial sequence of steps that:
    ///     1. Checks out the code from the repository
    ///     2. Applies a Remote Action or Shell Command (e.g. a docker image)
    /// </summary>
    public class GlobalConfiguration
    {
        private JobConfiguration _jobConfiguration;
        private LinkedList<Step.Step> _stepSequence;

        public GlobalConfiguration(JobConfiguration jobConfig)
        {
            _stepSequence = new LinkedList<Step.Step>();
            _jobConfiguration = jobConfig;
        }
        
        /// <summary>
        /// Adds a step that should be applied to all jobs in the pipeline
        /// </summary>
        /// <param name="step"></param>
        public void addStep(Step.Step step)
        {
            _stepSequence.AddLast(step);
        }
    }
}