namespace DSLPipeline.MetaModel.Step
{
     /// <summary>
     /// The Remote Action is equal to the concept 'uses' which contains an Action in Github Actions.
     /// An action is a reusable unit of code
     ///    
     /// Actions points to either JavaScript code or Docker containers
     ///    
     /// For more info See:
     /// https://help.github.com/en/actions/reference/workflow-syntax-for-github-actions#jobsjob_idstepsuses
     /// </summary>
    public class RemoteAction : Step
    {
        /// <summary>
        /// Gets the Path of the location of the Remote Action.
        ///
        /// Supported actions: github actions and docker actions.
        ///
        /// Examples:
        /// github action; actions/setup-node@v1
        /// pattern: {owner}/{repo}@{ref}
        ///    
        /// docker action; docker://alpine:3.8
        /// pattern: docker://{host}/{image}:{tag}
        /// </summary>
        public string Path { get; }
        
        public RemoteAction(string name, string path) : base(name)
        {
            Path = path;
        }
    }
}