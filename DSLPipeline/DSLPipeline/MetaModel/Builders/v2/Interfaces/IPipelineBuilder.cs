using System.Collections.Generic;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Jobs;
using DSLPipeline.MetaModel.Steps;

namespace DSLPipeline.MetaModel.Builders.v2.Interfaces
{
    /*
     * Valid pipeline constructs
     */
    public interface IPipelineBuilder : IAddJob, IBuild<Pipeline>
    {
        public IPipelineBuilder Pipeline(string name);

        public IPipelineBuilder TriggerOn(TriggerType triggerType);

        public IGlobalConfigBuilder AddGlobals();

        public Job Lookup(string jobId);

    }
    
    /*
     * Valid Global constructs
     */
    public interface IGlobalConfigBuilder : IAddStep, IAddJob, IBuild<GlobalConfiguration>
    {
        public IGlobalConfigBuilder SetEnvVar(string key, string value);
        public IGlobalConfigBuilder RunsOn(OperatingSystem os);
    }
    
    /*
     * Valid Job constructs
     */
    public interface IJobBuilder : IAddStep, IBuild<Job>
    {
        public IJobBuilder RunsOn(OperatingSystem os);
        public IJobBuilder SetEnvVar(string key, string value);

        public IJobBuilder DependsOn(string jobId);

        public IJobBuilder SetName(string jobName);
        public IJobBuilder SetId(string jobId);
    }

    /*
     * Valid Step constructs
     */
    public interface IStepsBuilder : IAddStep, IBuild<IList<Step>>
    {
        public IRemoteActionBuilder AsAction();
        public IShellCommandBuilder AsShell();
    }
    
    /*
     * Valid Action constructs
     */
    public interface IRemoteActionBuilder : IAddJob, IBuild<Step>, IAddStep
    {
        public IRemoteActionBuilder SetName(string name);
        public IRemoteActionBuilder Execute(string cmd);
    }

    /*
     * Valid Shell constructs
     */
    public interface IShellCommandBuilder : IAddJob, IBuild<Step>, IAddStep
    {
        public IShellCommandBuilder SetName(string name);
        public IShellCommandBuilder InDirectory(string directory);
        public IShellCommandBuilder Execute(string cmd);
    }

    public interface IAddJob
    {
        public IJobBuilder AddJob(string id);
    }

    public interface IAddStep
    {
        public IStepsBuilder AddStep(string name);
    }

    public interface IBuild<T>
    {
        public void Build();
        
        public T Collect();
    }
}