using DSLPipeline.MetaModel.Configuration;
using DSLPipeline.MetaModel.Steps;

namespace DSLTests
{
    public class TestModels
    {
        public static GlobalConfiguration GlobalConfiguration()
        {
            // Global configuration
            
            GlobalConfiguration globalConfig = new GlobalConfiguration(TestModels.GlobalJobConfiguration());
            
            globalConfig.AddStep(TestModels.GlobalCheckoutStep());
            globalConfig.AddStep(TestModels.GlobalDockerStep());
            globalConfig.AddStep(TestModels.GlobalCleanStep());

            return globalConfig;
        }

        public static JobConfiguration GlobalJobConfiguration()
        {
            JobConfiguration jobConfig = new JobConfiguration(OperatingSystem.UbuntuLatest);
            jobConfig.AddEnvVar("MYVAR1", "/usr/share/java");
            jobConfig.AddEnvVar("MYVAR2", "/usr/share/maven");

            return jobConfig;
        }

        public static RemoteAction GlobalCheckoutStep()
        {
            return new RemoteAction("checkout", "actions/checkout@v2");
        }

        public static RemoteAction GlobalDockerStep()
        {
            return new RemoteAction("Pull Docker Image", "docker://cfeicommon/maven-jdk8-fx:latest");
        }

        public static ShellCommand GlobalCleanStep()
        {
            ShellCommand cleanStep = new ShellCommand("Compile TANKS", "mvn clean");
            cleanStep.WorkDirectory = "./TANKS";

            return cleanStep;
        }

        public static ShellCommand CompileShellCmd()
        {
            ShellCommand compileStep = new ShellCommand("Compile TANKS", "mvn compile");
            compileStep.WorkDirectory = "./TANKS";

            return compileStep;
        }
        
        public static ShellCommand UnitTestShellCmd()
        {
            string stepname = "Execute unit tests";
            string cmd1 = "echo \"Running mvn verify\"";
            string cmd2 = "echo \"$MYVAR1 $MYVAR2\"";
            string cmd3 = "mvn verify";
            string workdir = "./TANKS";

            ShellCommand unitTestStep = new ShellCommand(stepname, 
                cmd1,
                cmd2,
                cmd3);
            unitTestStep.WorkDirectory = "./TANKS";

            return unitTestStep;
        }
        
        public static ShellCommand PackageShellCmd()
        {
            ShellCommand packageStep = new ShellCommand("Package TANKS", "mvn package");
            packageStep.WorkDirectory = "./TANKS";

            return packageStep;
        }

        public static ShellCommand InstallShellCmd()
        {
            ShellCommand installStep = new ShellCommand("Install Tanks", "mvn install");
            installStep.WorkDirectory = "./TANKS";

            return installStep;
        }
        
        
    }
}