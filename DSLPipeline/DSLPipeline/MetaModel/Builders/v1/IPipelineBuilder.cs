using DSLPipeline.MetaModel.Configuration;

namespace DSLPipeline.MetaModel.Builders.v1
{
    
    /// <summary>
    /// This interface represents a pipeline builder.
    /// The interface is very large and should be considered a code smell.
    /// Find a way to split this interface into smaller manageable interfaces.
    /// </summary>
    public interface IPipelineBuilder
    {
        /*
         * Pipeline constructs
         */
        public IPipelineBuilder Pipeline(string name);

        public IPipelineBuilder TriggerOn(TriggerType triggerType);

        public IPipelineBuilder ApplyGlobals();


        /*
         * Job constructs
         */
        public IPipelineBuilder AddJob(string name);

        public IPipelineBuilder SetEnvVar(string key, string value);

        public IPipelineBuilder RunsOn(OperatingSystem os);
        
        /*
         * Step constructs
         */
        public IPipelineBuilder AddStep(string name);

        public IPipelineBuilder WithAction();

        public IPipelineBuilder SetName(string name);

        public IPipelineBuilder WithShell();

        public IPipelineBuilder Execute(string cmd);

        public IPipelineBuilder InDirectory(string directory);
        
        public void GetConfig();

    }
}