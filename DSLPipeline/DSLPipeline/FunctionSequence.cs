using System;
using DSLPipeline.MetaModel;
using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Jobs;
using DSLPipeline.MetaModel.Steps;
using OperatingSystem = DSLPipeline.MetaModel.Configuration.OperatingSystem;

namespace DSLPipeline
{
    public class FunctionSequence
    {
        public static void Main(string[] args)
        {
            Pipeline pipe = new Pipeline("My pipe", TriggerType.Push);
            
            // Global configuration
            JobConfiguration jobConfig = new JobConfiguration(OperatingSystem.UbuntuLatest);
            jobConfig.AddEnvVar("MYVAR1", "/usr/share/java");
            jobConfig.AddEnvVar("MYVAR2", "/usr/share/maven");
            GlobalConfiguration globalConfig = new GlobalConfiguration(jobConfig);
            Step checkoutStep = new RemoteAction("checkout", "actions/checkout@v2");
            Step pullDockerImgStep = new RemoteAction("Pull Docker Image", "docker://cfeicommon/maven-jdk8-fx:latest");
            globalConfig.AddStep(checkoutStep);
            globalConfig.AddStep(pullDockerImgStep);
            
            // Individual Steps
            ShellCommand compileStep = new ShellCommand("Compile TANKS", "mvn compile");
            compileStep.WorkDirectory = "./TANKS";

            
            ShellCommand unitTestStep = new ShellCommand("Execute unit tests", 
                "echo \"Running mvn verify\"",
                                    "echo \"$MYVAR1 $MYVAR2\"",
                                    "mvn verify");
            unitTestStep.WorkDirectory = "./TANKS";
            
            
            ShellCommand packageStep = new ShellCommand("Package TANKS", "mvn package");
            packageStep.WorkDirectory = "./TANKS";
            
            
            ShellCommand installStep = new ShellCommand("Install Tanks", "mvn install");
            installStep.WorkDirectory = "./TANKS";
            
            
            // Individual Jobs
            Job compileJob = new Job("compile","Compile TANKS", jobConfig, compileStep);
            
            Job unitTestJob = new Job("unit-test","Unit test TANKS", jobConfig, unitTestStep);
            unitTestJob.AddDependency(compileJob);
            
            Job packageJob = new Job("package","Package TANKS", jobConfig, packageStep);
            packageJob.AddDependency(unitTestJob);
            
            Job installJob = new Job("install","Install TANKS", jobConfig, installStep);
            installJob.AddDependency(packageJob);
            
            pipe.AddJob(compileJob);
            pipe.AddJob(unitTestJob);
            pipe.AddJob(packageJob);
            pipe.AddJob(installJob);

            Console.WriteLine("Pipeline was created");
        }
    }
}