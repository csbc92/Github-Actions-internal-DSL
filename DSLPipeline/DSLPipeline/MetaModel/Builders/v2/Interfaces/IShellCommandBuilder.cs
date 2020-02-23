using DSLPipeline.MetaModel.Steps;

namespace DSLPipeline.MetaModel.Builders.v2.Interfaces
{
    /*
     * Valid Shell constructs
     */
    public interface IShellCommandBuilder : IAddJob, IBuild<Step>, IAddStep
    {
        public IShellCommandBuilder SetName(string name);
        public IShellCommandBuilder InDirectory(string directory);
        public IShellCommandBuilder Execute(string cmd);
    }
}