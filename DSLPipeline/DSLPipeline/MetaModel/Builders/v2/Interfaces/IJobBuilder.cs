using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Jobs;

namespace DSLPipeline.MetaModel.Builders.v2.Interfaces
{
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
}