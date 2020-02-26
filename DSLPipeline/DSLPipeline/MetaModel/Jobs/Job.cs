using System;
using System.Collections.Generic;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Steps;

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
        private LinkedList<Step> _steps;
        
        public string Id { get; }
        public string Name { get; }

        public JobConfiguration JobConfiguration => _jobConfiguration;

        public ISet<Job> Dependencies => new HashSet<Job>(_dependencies);
        
        public ICollection<Step> Steps => new LinkedList<Step>(_steps);

        public Job(string id, string name, JobConfiguration jobConfiguration, params Step[] steps)
        {
            Id = id;
            Name = name;
            _dependencies = new HashSet<Job>();
            _jobConfiguration = jobConfiguration;
            _steps = new LinkedList<Step>();

            foreach (var step in steps)
            {
                AddStep(step);
            }
        }

        public void AddDependency(Job job)
        {
            if (job.Equals(this))
                throw new ArgumentException("A Job cannot depend on itself");

            if (job == null)
                throw new ArgumentNullException(nameof(job));

 
            _dependencies.Add(job);
        }

        /// <summary>
        /// Adds the given step to the job. The step can be appended or prepended. Default option is to append.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="append">True if append, False if prepend.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddStep(Step step, bool append = true)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));
            if (_steps.Contains(step))
                throw new ArgumentException("The step is already added to the sequence");

            if (append)
            {
                _steps.AddLast(step);
            }
            else // Prepend
            {
                _steps.AddFirst(step);
            }
            
        }

        public void AddEnvironmentVariable(string name, string value)
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

            return this.Id.Equals(other.Id) || this.Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            int result = 17;

            result = result * 31 + this.Name.GetHashCode();
            result = result * 31 + this.Id.GetHashCode();

            return result;
        }
    }
}