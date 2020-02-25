using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DSLPipeline.Builders.v2;
using DSLPipeline.Builders.v2.Implementations;
using DSLPipeline.Builders.v2.Interfaces;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Steps;

namespace DSLTests
{
    public class Tests
    {
        private IPipelineBuilder _pipelineBuilder;
        private IStepsBuilder _stepsBuilder;
        
        [SetUp]
        public void Setup()
        {
            _pipelineBuilder = new PipelineBuilderImpl();
            _stepsBuilder = new StepsBuilderImpl(_pipelineBuilder);
        }

        [Test]
        public void IShellBuilderTest()
        {

            string name = "My Shell Command";
            string workDir = "/myworkdir";
            string exec1 = "echo hello";
            string exec2 = "echo world";
            
            IShellCommandBuilder b = new ShellCommandBuilderImpl(_stepsBuilder, _pipelineBuilder);
            b.SetName(name)
                .InDirectory(workDir)
                .Execute(exec1)
                .Execute(exec2)
                .Build();
            ShellCommand actual = (ShellCommand)b.Collect();
            
            
            ShellCommand expected = new ShellCommand(name, exec1, exec2);
            expected.WorkDirectory = workDir;
            
            
            Assert.AreEqual(expected.WorkDirectory, actual.WorkDirectory);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.getCommandLines(), actual.getCommandLines());
        }
        
        [Test]
        public void IActionBuilderTest()
        {

            string name = "My Remote Action";
            string path1 = "docker://mypath";
            string path2 = "actions/checkout@v2";
            
            IRemoteActionBuilder b = new RemoteActionBuilderImpl(_stepsBuilder, _pipelineBuilder);
            b.SetName(name)
                .Execute(path1)
                .Execute(path2) // Should override
                .Build();
            RemoteAction actual = (RemoteAction)b.Collect();
            
            
            RemoteAction expected = new RemoteAction(name, path2);
            
            
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Path, actual.Path);
        }


        [Test]
        public void IStepsBuilderTest()
        {
            string step1 = "Step1";
            string cmd1 = "echo line1";
            string cmd2 = "echo line2";
            string workDir1 = "./MYWORKDIR1";
            
            string step2 = "Step2";
            string action2 = "docker://cfeidocker/jdk8-mvn-jfx";
            
            string step3 = "Step3";
            string workdir3 = "./MYWORKDIR3";
            string cmd3 = "echo \"HELLO $WORLD\"";
            
            
            IStepsBuilder stepsBuilder = new StepsBuilderImpl(_pipelineBuilder);
            stepsBuilder
                .AddStep(step1)
                    .AsShell()
                        .Execute(cmd1)
                        .Execute(cmd2)
                        .InDirectory(workDir1)
                .AddStep(step2)
                    .AsAction()
                        .Execute(action2)
                .AddStep(step3)
                    .AsShell()
                    .InDirectory(workdir3)
                    .Execute(cmd3)
                    .Execute("mvn clean");
            
            // Important to call build on the StepsBuilder and not on other builders
            // since we are building from top to bottom.
            stepsBuilder.Build(); 

            IList<Step> steps = stepsBuilder.Collect();

            int expectedStepsCount = 3;
            
            Assert.AreEqual(expectedStepsCount, steps.Count);
        }

        [Test]
        public void IGlobalConfigBuilderTest()
        {
            string step1 = "Step1";
            string cmd1 = "echo line1";
            string cmd2 = "echo line2";
            string workDir1 = "./MYWORKDIR1";
            
            string step2 = "Step2";
            string action2 = "docker://cfeidocker/jdk8-mvn-jfx";
            
            string step3 = "Step3";
            string workdir3 = "./MYWORKDIR3";
            string cmd3 = "echo \"HELLO $WORLD\"";
            
            IGlobalConfigBuilder b = new GlobalConfigurationBuilderImpl(_pipelineBuilder);
                b.RunsOn(OperatingSystem.Ubuntu1604)
                    .SetEnvVar("MYVAR1", "HELLO")
                    .SetEnvVar("MYVAR2", "WORLD")
                .AddStep(step1)
                    .AsShell()
                        .Execute(cmd1)
                        .Execute(cmd2)
                        .InDirectory(workDir1)
                .AddStep(step2)
                    .AsAction()
                        .Execute(action2)
                .AddStep(step3)
                    .AsShell()
                    .InDirectory(workdir3)
                    .Execute(cmd3)
                    .Execute("mvn clean");
            b.Build();
            
            GlobalConfiguration actual = b.Collect();
            
            
            b = new GlobalConfigurationBuilderImpl(_pipelineBuilder);
            b.RunsOn(OperatingSystem.UbuntuLatest)
                .SetEnvVar("MYVAR1", "HELLO")
                .SetEnvVar("MYVAR2", "WORLD")
                .AddStep(step1)
                    .AsShell()
                        .Execute("lol")
                        .Execute("lol2")
                        .InDirectory("/home/ccl/")
                    .AddStep(step2)
                        .AsAction()
                        .Execute("docker://lmaoqweqwe")
                    .AddStep(step3)
                        .AsAction()
                        .Execute("actions/checkout@v2");
            b.Build();
            actual = b.Collect();

        }
    }
}