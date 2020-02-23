using DSLPipeline.MetaModel.Configuration;

namespace DSLPipeline.MetaModel.Builders.v2.Interfaces
{
    /*
     * Valid Global constructs
     */
    public interface IGlobalConfigBuilder : IAddStep, IAddJob, IBuild<GlobalConfiguration>
    {
        public IGlobalConfigBuilder SetEnvVar(string key, string value);
        public IGlobalConfigBuilder RunsOn(OperatingSystem os);
    }
}