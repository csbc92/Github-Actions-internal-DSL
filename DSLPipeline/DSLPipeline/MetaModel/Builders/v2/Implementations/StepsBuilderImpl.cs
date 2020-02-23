using System.Collections.Generic;
using System.Linq;
using DSLPipeline.MetaModel.Builders.v2.Interfaces;
using DSLPipeline.MetaModel.Steps;

namespace DSLPipeline.MetaModel.Builders.v2.Implementations
{
    public class StepsBuilderImpl : IStepsBuilder
    {
        private IPipelineBuilder _pipelineBuilder;
        private IRemoteActionBuilder _remoteActionBuilder;
        private IShellCommandBuilder _shellCommandBuilder;
        private IList<Step> _currentSteps;
        private string _currentStepName;
        private Step _currentStep;

        public StepsBuilderImpl(IPipelineBuilder pipelineBuilder)
        {
            _pipelineBuilder = pipelineBuilder;
            _currentSteps = new List<Step>();
        }
        
        public IStepsBuilder AddStep(string name)
        {
            Build(); // Build existing Step under construction
            Flush(); // Flush existing builders
            
            _currentStepName = name;
            return this;
        }

        public IRemoteActionBuilder AsAction()
        {
            _remoteActionBuilder = new RemoteActionBuilderImpl(this, _pipelineBuilder);
            _remoteActionBuilder.SetName(_currentStepName);
            return _remoteActionBuilder;
        }

        public IShellCommandBuilder AsShell()
        {
            _shellCommandBuilder = new ShellCommandBuilderImpl(this, _pipelineBuilder);
            _shellCommandBuilder.SetName(_currentStepName);
            return _shellCommandBuilder;
        }

        public void Build()
        {
            _currentStep = BuildCurrentStep();

            if (_currentStep != null)
            {
                _currentSteps.Add(_currentStep);
            }
        }

        private void Flush()
        {
            _shellCommandBuilder = null;
            _remoteActionBuilder = null;
            _currentStep = null;
        }

        private Step BuildCurrentStep()
        {
            Step currentStep = null;
            
            if (_remoteActionBuilder != null)
            {
                _remoteActionBuilder.Build();
                currentStep = _remoteActionBuilder.Collect();
            }
            else if (_shellCommandBuilder != null)
            {
                _shellCommandBuilder.Build();
                currentStep = _shellCommandBuilder.Collect();
            }
            
            return currentStep;
        }
        
        public IList<Step> Collect()
        {
            return _currentSteps.ToList();
        }
    }
}