using DSLPipeline.MetaModel;
using DSLPipeline.MetaModel.Builders.v2.Interfaces;
using DSLPipeline.MetaModel.Builders.v2.Implementations;
using DSLPipeline.MetaModel.Configuration;
using NUnit.Framework;

namespace DSLTests
{
    public class FullPipelineTest
    {

        [SetUp]
        public void Setup()
        {
            
        }
        
        [Test]
        public void InstantiatePipelineTest()
        {
            IPipelineBuilder builder = new PipelineBuilderImpl();
            
            builder.Pipeline("myPipeline").
                TriggerOn(TriggerType.Push).
                AddGlobals().
                    RunsOn(OperatingSystem.UbuntuLatest).
                    SetEnvVar("MY_ENV_VAR", "HELLO WORLD!").
                    AddStep("Default Checkout step").
                        AsAction().
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
                AddJob("unittest").
                    AddStep("Unit Test Step").
                        AsShell().
                            Execute("mvn verify").
                            InDirectory("./TANKS");
            
            builder.Build();
            Pipeline p = builder.Collect();
        }
    }
}