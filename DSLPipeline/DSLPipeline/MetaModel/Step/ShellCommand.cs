using System.Collections.Generic;

namespace DSLPipeline.MetaModel.Step
{
    /// <summary>
    /// The Shell Command is equal to the concept 'run' in Github Actions.
    /// An action is a reusable unit of code
    ///    
    /// For more info See:
    /// https://help.github.com/en/actions/reference/workflow-syntax-for-github-actions#jobsjob_idstepsrun
    /// </summary>
    public class ShellCommand : Step
    {
        /// <summary>
        /// Gets or sets the Work-directory under where commands are executed
        /// </summary>
        public string WorkDirectory { get; set; }

        private List<string> _commandLines;
        
        public ShellCommand(string name, params string[] commandLines) : base(name)
        {
            _commandLines = new List<string>();

            foreach (var cmd in commandLines)
            {
                addCommandLine(cmd);
            }
        }

        /// <summary>
        /// Adds a command to be executed as part of this step
        /// </summary>
        /// <param name="cmd">A one-line shell command</param>
        public void addCommandLine(string cmd)
        {
            _commandLines.Add(cmd);
        }

        /// <summary>
        /// Immutable array of command lines
        /// </summary>
        /// <returns>A copy of command lines</returns>
        public string[] getCommandLines()
        {
            return _commandLines.ToArray();
        }
    }
}