using System.Collections.Generic;
using DSLPipeline.MetaModel.Steps;

namespace DSLPipeline.Builders.v2.Interfaces
{
    /*
     * Valid Step constructs
     */
    public interface IStepsBuilder : IAddStep, IBuild<IList<Step>>
    {
        public IRemoteActionBuilder AsAction();
        public IShellCommandBuilder AsShell();
    }
}