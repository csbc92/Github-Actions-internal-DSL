using System;
using DSLPipeline.Builders.v2.Implementations;
using DSLPipeline.Builders.v2.Interfaces;
using DSLPipeline.Codegenerators;
using DSLPipeline.MetaModel;
using NUnit.Framework;
using OperatingSystem = DSLPipeline.MetaModel.Configuration.OperatingSystem;

namespace DSLTests.CodeGenTests
{
    public class YAMLGeneratorTest
    {
        private Pipeline _pipeline;
        
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GenerateYAML1()
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
            _pipeline = builder.Collect();
            
            PipelineCodeGen codeGen = new PipelineCodeGen(_pipeline, 2);
            string YAML = codeGen.Generate();
        }
        
                [Test]
        public void GenerateYAML2()
        {
            IPipelineBuilder builder = new PipelineBuilderImpl();

            string workDir = "./TANKS";
            
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
                            SetName("Maven Clean").
                            Execute("echo \"My Global Job\"").
                            Execute("echo \"Value of global env var: \" $MY_ENV_VAR").
                            Execute("mvn clean").
                            InDirectory(workDir).
                AddJob("compile").
                    AddStep("Compile Step").
                        AsShell().
                            Execute("mvn compile").
                            InDirectory(workDir).
                AddJob("unit-test").
                    RunsOn(OperatingSystem.Ubuntu1604).
                    DependsOn("compile").
                    AddStep("Unit Test Step").
                        AsShell().
                            Execute("mvn verify").
                            InDirectory(workDir).
                AddJob("package").
                    RunsOn(OperatingSystem.Ubuntu1804).
                    DependsOn("unit-test").
                    SetEnvVar("MY_ENV_VAR", "HELLO FYN").
                    SetEnvVar("LOCAL_VAR", "HELLO ODENSE").
                    AddStep("Maven package").
                        AsShell().
                            Execute("mvn package").
                            Execute("echo \"Value of overridden global env var: \" $MY_ENV_VAR").
                            Execute("echo \"Value of local env var: \" $LOCAL_VAR").
                            InDirectory(workDir);
            
            builder.Build();
            _pipeline = builder.Collect();
            
            PipelineCodeGen codeGen = new PipelineCodeGen(_pipeline, 2);
            string YAML = codeGen.Generate();
            Console.WriteLine(YAML);
        }
    }
}