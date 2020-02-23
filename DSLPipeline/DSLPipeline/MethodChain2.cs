using DSLPipeline.MetaModel;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Builders.v2.Interfaces;

namespace DSLPipeline
{
    public class MethodChain2
    {
        public MethodChain2()
        {
            IPipelineBuilder builder = null;
            
            builder.Pipeline("myPipeline").
                TriggerOn(TriggerType.Push).
                AddGlobals().
                    RunsOn(OperatingSystem.UbuntuLatest).
                    SetEnvVar("MY_ENV_VAR", "HELLO WORLD!").
                    AddStep("Default Checkout step").
                        AsAction().
                            SetName("Checkout").
                            Execute("actions/checkout@v2").
                    AddStep("Default clean step").
                        AsShell().
                            Execute("echo \"My Global Job\"").
                            Execute("mvn clean").
                            InDirectory("./TANKS").
                AddJob("compile").
                    AddStep("Compile Step").
                        AsShell().
                            Execute("mvn compile").
                            InDirectory("./TANKS").
                AddJob("compile").
                    AddStep("Compile Step").
                        AsShell().
                            Execute("mvn compile").
                            InDirectory("./TANKS");
            
            builder.Build();
            builder.Collect();
        }
    }
}