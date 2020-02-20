using System;
using System.Collections.Generic;
using DSLPipeline.MetaModel.Configuration;

namespace DSLPipeline.MetaModel.Jobs
{
    /// <summary>
    /// A Job is equal to the concept of Job in Github Actions.
    ///
    /// A job can contain a sequence of Steps that are executed as part of the job.
    /// 
    /// A job can depend on other jobs before it is allowed to be executed. Concept is called 'Needs' in Github Actions.
    ///
    /// See link for more info: https://help.github.com/en/actions/reference/workflow-syntax-for-github-actions#jobs
    /// </summary>
    public class Job
    {
        private ISet<Job> _dependencies;

        private JobConfiguration _jobConfiguration;

        // Stored in a linked list, since steps are mostly appended first/last or accessed first/last or deleted 
        private LinkedList<Step.Step> _steps;
        
        public string ID { get; }
        public string Name { get; }

        public Job(string id, string name, JobConfiguration jobConfiguration, params Step.Step[] steps)
        {
            ID = name;
            Name = name;
            _dependencies = new HashSet<Job>();
            _jobConfiguration = jobConfiguration;
            _steps = new LinkedList<Step.Step>();

            foreach (var step in steps)
            {
                addStep(step);
            }
        }

        public void addDependency(Job job)
        {
            if (job.Equals(this))
                throw new ArgumentException("A Job cannot depend on itself");

            if (job == null)
                throw new ArgumentNullException(nameof(job));

 
            _dependencies.Add(job);
        }

        public void addStep(Step.Step step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));
            if (_steps.Contains(step))
                throw new ArgumentException("The step is already added to the sequence");

            _steps.AddLast(step);
        }

        public void addEnvironmentVariable(string name, string value)
        {
            _jobConfiguration.AddEnvVar(name, value);
        }

        /// <summary>
        /// A Job is considered to be equal if their Name and ID match.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if ((obj == null) || this.GetType() != obj.GetType())
            {
                return false;
            }

            Job other = (Job) obj;

            return this.Name.Equals(other.Name) && this.ID.Equals(other.ID);
        }

        public override int GetHashCode()
        {
            int result = 17;

            result = result * 31 + this.Name.GetHashCode();
            result = result * 31 + this.ID.GetHashCode();

            return result;
        }
    }
}