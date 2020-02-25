using DSLPipeline.MetaModel.Steps;

namespace DSLPipeline.Builders.v2.Interfaces
{
    /*
     * Valid Action constructs
     */
    public interface IRemoteActionBuilder : IAddJob, IBuild<Step>, IAddStep
    {
        public IRemoteActionBuilder SetName(string name);
        public IRemoteActionBuilder Execute(string cmd);
    }
}