using DSLPipeline.MetaModel;
using DSLPipeline.Builders.v1;
using DSLPipeline.MetaModel.Configuration;

namespace DSLPipeline
{
    public class MethodChain
    {
        public MethodChain()
        {
            IPipelineBuilder builder = null;
            
            builder.Pipeline("myPipeline").
                TriggerOn(TriggerType.Push).
                ApplyGlobals().
                    RunsOn(OperatingSystem.UbuntuLatest).
                    SetEnvVar("MY_ENV_VAR", "HELLO WORLD!").
                    AddStep("Default step").
                        WithAction().
                            SetName("Checkout").
                            Execute("actions/checkout@v2").
                        WithShell().
                            Execute("echo \"My Global Job\"").
                            Execute("mvn clean").
                            InDirectory("./TANKS").
                AddJob("compile").
                    AddStep("Compile Step").
                        WithShell().
                            Execute("mvn compile").
                            InDirectory("./TANKS").
                AddJob("compile").
                    AddStep("Compile Step").
                        WithShell().
                            Execute("mvn compile").
                            InDirectory("./TANKS").
                
                GetConfig();
        }
    }
}