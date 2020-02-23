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

        [Test]
        public void InstantiateTANKSPipelineTest()
        {
            IPipelineBuilder builder = new PipelineBuilderImpl();

            string tanksDir = "./TANKS";
            string checkoutAction = "actions/checkout@v2";
            string dockerAction = "docker://cfeicommon/maven-jdk8-fx:latest";
            
            builder.Pipeline("CI")
                .TriggerOn(TriggerType.Push)
                
                // BUG: The builder crashes if there are no globals added
                .AddGlobals().
                RunsOn(OperatingSystem.UbuntuLatest).
                SetEnvVar("MY_ENV_VAR", "HELLO WORLD!").
                AddStep("Default Checkout step").
                AsAction().
                Execute("actions/checkout@v2")
                
                .AddJob("compile")
                    .RunsOn(OperatingSystem.UbuntuLatest)
                    .AddStep("Checkout repo")
                        .AsAction()
                            .Execute(checkoutAction)
                    .AddStep("Pull Docker Image")
                        .AsAction()
                            .Execute(dockerAction)
                    .AddStep("Compile Tanks")
                        .AsShell()
                            .Execute("mvn compile")
                            .InDirectory(tanksDir)
                .AddJob("unit-test")
                    .RunsOn(OperatingSystem.UbuntuLatest)
                    .DependsOn("compile")
                    .AddStep("Checkout repo")
                        .AsAction()
                            .Execute(checkoutAction)
                    .AddStep("Pull Docker Image")
                        .AsAction()
                            .Execute(dockerAction)
                    .AddStep("Execute unit test TANKS")
                        .AsShell()
                            .Execute("Running mvn verify")
                            .Execute("mvn verify")
                            .InDirectory(tanksDir)
                .AddJob("package")
                    .RunsOn(OperatingSystem.UbuntuLatest)
                    .DependsOn("unit-test")
                    .AddStep("Checkout repo")
                        .AsAction()
                        .Execute(checkoutAction)
                    .AddStep("Pull Docker Image")
                        .AsAction()
                        .Execute(dockerAction)
                    .AddStep("Package TANKS")
                        .AsShell()
                            .Execute("mvn package")
                            .InDirectory(tanksDir)
                .AddJob("install")
                    .RunsOn(OperatingSystem.UbuntuLatest)
                    .DependsOn("package")
                    .AddStep("Checkout repo")
                        .AsAction()
                            .Execute(checkoutAction)
                    .AddStep("Pull Docker Image")
                        .AsAction()
                            .Execute(dockerAction)
                    .AddStep("Install TANKS")
                        .AsShell()
                            .Execute("mvn install")
                            .InDirectory(tanksDir);
            
            builder.Build();
            Pipeline p =builder.Collect();

        }
    }
}