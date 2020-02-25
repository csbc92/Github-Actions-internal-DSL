using DSLPipeline.MetaModel;
using DSLPipeline.MetaModel.Jobs;

namespace DSLPipeline.Builders.v2.Interfaces
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
}