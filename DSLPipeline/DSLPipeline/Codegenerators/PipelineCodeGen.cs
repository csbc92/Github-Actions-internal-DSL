using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSLPipeline.MetaModel;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Jobs;
using DSLPipeline.MetaModel.Steps;

namespace DSLPipeline.Codegenerators
{
    
    public class PipelineCodeGen
    {
        private Pipeline _pipeline;
        private StringBuilder _sb;
        private int _indent;
        private char _indentChar = ' ';

        /// <summary>
        /// Constructs a YAML code generator
        /// </summary>
        /// <param name="pipeline">The model to generate</param>
        /// <param name="indent">The space indentations to use e.g. 2 or 4, ..</param>
        public PipelineCodeGen(Pipeline pipeline, int indent = 2)
        {
            _pipeline = pipeline;
            _indent = indent;
            _sb = new StringBuilder();

            PrepareModel();
        }
        
        public int Indent
        {
            get => _indent;
            set => _indent = value;
        }

        private void PrepareModel()
        {
            // Add the Global Configuration to the Jobs in the pipeline
            // This step is specific for this generator.
            GlobalConfiguration g = _pipeline.GlobalConfiguration;

            if (g == null)
                return; // Nothing to apply

            
            // Apply global settings for each Job in the pipeline
            foreach (var job in _pipeline.Jobs)
            {
                // Apply global environment variables to the job
                if (g.JobConfiguration != null)
                {
                    foreach (var envVar in g.JobConfiguration.EnvironmentVariables)
                    {
                        job.JobConfiguration.AddEnvVar(envVar.Key, envVar.Value, replace: false);
                    }

                    // Apply the global OS if not already set
                    if (job.JobConfiguration.OperatingSystem == null)
                    {
                        job.JobConfiguration.OperatingSystem = g.JobConfiguration.OperatingSystem;
                    }
                }

                // Apply global (pre)steps to the job
                Step[] globalSteps = _pipeline.GlobalConfiguration.StepSequence;
                if (globalSteps != null)
                {
                    // Traverse steps backwards to preserve the order when prepended.
                    for (int i = globalSteps.Length - 1; i >= 0; i--)
                    {
                        job.AddStep(globalSteps[i], append: false); // prepend the step
                    }
                }
            }
        }

        public string Generate()
        {
            _sb.Clear();
            
            GeneratePipelineName();
            AppendNewLine(2);
            GenerateTriggers();
            AppendNewLine(2);
            GenerateJobs();

            return _sb.ToString();
        }

        private void AddIndent(int times = 1)
        {
            for (int i = 0; i < times; i++)
                _sb.Append(_indentChar, _indent);
        }
        
        private void AppendNewLine(int count)
        {
            for (int i = 0; i < count; i++)
                _sb.Append("\n");
        }

        private void GeneratePipelineName()
        {
            _sb.Append("name: ").Append(_pipeline.Name);
        }

        private void GenerateTriggers()
        {
            _sb.Append("on: [");

            TriggerType[] triggers = _pipeline.TriggerTypes.ToArray();

            for (int i = 0; i < triggers.Length; i++)
            {
                _sb.Append(triggers[i].Name);

                if ((i+1) < triggers.Length) // Append if there are more elements to add
                    _sb.Append(", ");
            }

            _sb.Append("]");
        }

        private void GenerateJobs()
        {
            _sb.Append("jobs:");
            AppendNewLine(1);

            foreach (var job in _pipeline.Jobs)
            {
                AppendNewLine(1);
                AddIndent();
                _sb.Append(job.Id).Append(":");
                AppendNewLine(1);
                
                AddIndent(2);
                _sb.Append("name: ").Append(job.Name);
                
                AppendNewLine(1);
                AddIndent(2);
                _sb.Append("runs-on: [").Append(job.JobConfiguration.OperatingSystem.Name).Append("]");

                GenerateDependencies(job);
                GenerateEnvironmentVars(job);
                GenerateSteps(job);   
            }
        }

        private void GenerateDependencies(Job job)
        {
            Job[] dependencies = job.Dependencies?.ToArray();

            if (dependencies != null && dependencies.Length > 0)
            {
                AppendNewLine(1);
                AddIndent(2);
                _sb.Append("needs: [");

                for (int i = 0; i < dependencies.Length; i++)
                {
                    _sb.Append(dependencies[i].Id);

                    if ((i + 1) < dependencies.Length) // More elements to come - Append a comma
                    {
                        _sb.Append(", ");
                    }
                }

                _sb.Append("]");
            }
        }
        
        private void GenerateEnvironmentVars(Job job)
        {
            IDictionary<string, string> envVars = job.JobConfiguration.EnvironmentVariables;

            if (envVars != null)
            {
                AppendNewLine(1);
                AddIndent(2);
                _sb.Append("env: ");

                foreach (var keyValPair in envVars)
                {
                    AppendNewLine(1);
                    AddIndent(3);
                    _sb.Append(keyValPair.Key)
                        .Append(": ").Append("\"")
                        .Append(keyValPair.Value)
                        .Append("\"");
                }
            }
        }
        
        private void GenerateSteps(Job job)
        {
            if (job.Steps != null)
            {
                AppendNewLine(1);
                AddIndent(2);
                _sb.Append("steps:");

                foreach (var step in job.Steps)
                {
                    AppendNewLine(1);
                    AddIndent(3);
                    _sb.Append("- ");

                    if (step is RemoteAction)
                    {
                        GenerateRemoteAction((RemoteAction)step);

                    }
                    else if (step is ShellCommand)
                    {
                        GenerateShellCommand((ShellCommand)step);
                    }
                }
            }
        }

        private void GenerateRemoteAction(RemoteAction remoteAction)
        {
            _sb.Append("uses: ").Append(remoteAction.Path);
            AppendNewLine(1);
            AddIndent(4);
            _sb.Append("name: ").Append(remoteAction.Name);
        }
        
        private void GenerateShellCommand(ShellCommand shellCmd)
        {
            _sb.Append("name: ").Append(shellCmd.Name);
            AppendNewLine(1);
            AddIndent(4);
                        
            string workDir = shellCmd.WorkDirectory;
            if (workDir != null)
            {
                _sb.Append("working-directory: ").Append(workDir);
            }

            string[] cmdLines = (shellCmd.getCommandLines());
            if (cmdLines != null)
            {
                AppendNewLine(1);
                AddIndent(4);
                _sb.Append("run: ");
                            
                if (cmdLines.Length > 1) // Multi-line case
                {
                    _sb.Append("|");

                    for (int i = 0; i < cmdLines.Length; i++)
                    {
                        AppendNewLine(1);
                        AddIndent(5);
                        _sb.Append(cmdLines[i]);
                    }
                }
                else // Single line case
                {
                    string cmd = cmdLines[0];
                    _sb.Append(cmd);
                }
            }
        }
    }
}