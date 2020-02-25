using System.Collections.Generic;
using System.Linq;
using DSLPipeline.Builders.v2.Interfaces;
using DSLPipeline.MetaModel.Steps;

namespace DSLPipeline.Builders.v2.Implementations
{
    public class ShellCommandBuilderImpl : IShellCommandBuilder
    {
        private IStepsBuilder _parentBuilder;
        private IPipelineBuilder _parentPipeline;
        private string _name;
        private IList<string> _commands;
        private string _directory;
        private ShellCommand _currentCommand;

        public ShellCommandBuilderImpl(IStepsBuilder parentBuilder, IPipelineBuilder parentPipeline)
        {
            _parentBuilder = parentBuilder;
            _parentPipeline = parentPipeline;
            _commands = new List<string>();
        }
        
        public IJobBuilder AddJob(string id)
        {
            return _parentPipeline.AddJob(id);
        }

        public IStepsBuilder AddStep(string name)
        {
            return _parentBuilder.AddStep(name);
        }

        public IShellCommandBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        public IShellCommandBuilder InDirectory(string directory)
        {
            _directory = directory;
            return this;
        }

        public IShellCommandBuilder Execute(string cmd)
        {
            _commands.Add(cmd);
            return this;
        }

        public void Build()
        {
            _currentCommand = new ShellCommand(_name, _commands.ToArray());
            _currentCommand.WorkDirectory = _directory;
        }

        public Step Collect()
        {
            return _currentCommand;
        }
    }
}