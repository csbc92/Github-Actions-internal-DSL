using DSLPipeline.MetaModel.Builders.v2.Interfaces;
using DSLPipeline.MetaModel.Steps;

namespace DSLPipeline.MetaModel.Builders.v2.Implementations
{
    public class RemoteActionBuilderImpl : IRemoteActionBuilder
    {
        private IStepsBuilder _parentBuilder;
        private IPipelineBuilder _parentPipeline;
        private string _name;
        private string _path;
        private RemoteAction _currentRemoteAction;

        public RemoteActionBuilderImpl(IStepsBuilder parentBuilder, IPipelineBuilder parentPipeline)
        {
            _parentBuilder = parentBuilder;
            _parentPipeline = parentPipeline;
        }
        
        public IJobBuilder AddJob(string id)
        {
            return _parentPipeline.AddJob(id);
        }

        public IRemoteActionBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        public IRemoteActionBuilder Execute(string path)
        {
            _path = path;
            return this;
        }

        public void Build()
        {
            _currentRemoteAction = new RemoteAction(_name, _path);
        }

        public Step Collect()
        {
            return _currentRemoteAction;
        }

        public IStepsBuilder AddStep(string name)
        {
            return _parentBuilder.AddStep(name);
        }
    }
}