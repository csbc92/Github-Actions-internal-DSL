# Github Actions internal DSL
This repository (https://github.com/csbc92/Github-Actions-internal-DSL) contains an internal DSL (written in C#) for creating pipelines with Github Actions
<br>
See more: https://github.com/features/actions
<br>
YAML specification: https://help.github.com/en/actions/reference/workflow-syntax-for-github-actions

The development of the internal DSL was part of an assignment for the Master's course, Model-Driven Software Development, in the Software Engineering program at University of Southern Denmark.


# Purpose of the DSL
This internal DSL aims to make it easier to compose Github Action pipelines and to enhance on the available features.

Added features which is not available in Github Actions:

* Global Steps - allows to create steps that are prepended to all defined jobs instead of repeating trivial steps for each job (for instance 'checkout').

* Global operating system - allows to make all jobs use a specific OS by defining it once in a global section.

* Global environment variables - allows to define environment variables once which applies to all jobs.

Individual jobs can override the global configuration.

# What does the DSL do?
The DSL builds a meta model instance of a pipeline and uses this instance to generate YAML output, which can be executed by the Github actions environment.

# How does it work?
The internal DSL uses the technique of Method Chaining which is a convinient way of programming because developers are notified by the IDE of the allowed constructs. This approach should give the developer a more streamlined experience when using the DSL. The DSL contains multiple builder interfaces and an appropriate builder interface is returned based on the previous method call in the chain. Each method call in the chain subsequently adds information to the builders about how the underlying meta model should be constructed. The meta model is constructed in a bottom-up apprach where the highest level of abstraction (the pipeline) calls the Build()-method on its nested builders, which calls to their nested builders until we hit the bottom and can create the smallest parts. We can then start building larger parts from smaller parts. This pattern is known as the Construction builder pattern (https://www.martinfowler.com/dslCatalog/constructionBuilder.html). The choice of method chaining was based sololy on giving convenience to developers when they write in the DSL. Other approaches are definitely possible to give the language more structure e.g. Nested Functions, Lambdas or Hybrids.

The meta model instance is created and is then passed to a Code Generator which prepares the meta model by manipulating it to suit the output specification. For example, Global Configurations are propagated into each individual job because the concept Global Configuration does not exist in the Github Actions specification. The Code Generator is simply creating a long YAML string using .NET's built-in StringBuilder.


**Key techniques used:** Method chaining, Builder pattern, Construction Builder pattern, Context variables, Lookup table (for creating dependencies between jobs).


# Structure of the source code
DSLPipeline/MetaModel - Contains the meta model of the pipeline

DSLPipeline/Builders/v2/Interfaces - contains all the interfaces used by the internal DSL

DSLPipeline/Builders/v2/Implementations - contains implementations of the builder interfaces

DSLPipeline/CodeGenerators - contains the YAML code generator

DSLPipeline/DSLTests - contains a few *incomplete* unit tests


# Limitations
It is important to notice that there are limitations to the DSL. This means that the DSL does not (yet) implement the full specification of the Github actions API. In an ideal world the meta model should be abstracted to a level where the meta model is independent of the underlying platform. This remains an usolved challenge because there is no general agreement of which concepts that constitutes an automated pipeline. Different platforms may share the general idea of automated pipelines, but they do not share the naming of the concepts. Hence, the development of an abstracted meta model would require knowledge from multiple domain experts.



# Demonstration
The Github repository https://github.com/csbc92/TANKS contains a game written in Java. The process of building this game can be automated with a pipeline for Github actions.

The programs that are written in the internal DSL should be self-explanatory to users that have a background in Computer Science, Software Engineering or similar. It is recommended to have some knowledge of Continuous Integration, C#, Maven and \*nix systems.

## Example 1
Constructs a pipeline that looks like this: Compile -> Unit test -> Package -> Install

### Generating the meta model instance

```C#
IPipelineBuilder builder = new PipelineBuilderImpl();

string workDir = "./TANKS";

builder.Pipeline("myPipeline").
// Global section
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
                Execute("echo \"Value of global env var: \" $MY_ENV_VAR").
                Execute("mvn clean").
                InDirectory(workDir).

// Individual jobs
    AddJob("compile").
        SetName("Compile").
        AddStep("Compile Step").
            AsShell().
                Execute("mvn compile").
                InDirectory(workDir).
    AddJob("unit-test").
        SetName("Unit test").
        RunsOn(OperatingSystem.Ubuntu1604). // Override the Global configuration's RunsOn
        DependsOn("compile").
        AddStep("Unit Test Step").
            AsShell().
                Execute("mvn verify").
                InDirectory(workDir).
    AddJob("package").
        SetName("Package").
        RunsOn(OperatingSystem.Ubuntu1804). // Override the Global configuration's RunsOn
        DependsOn("unit-test").
        SetEnvVar("MY_ENV_VAR", "HELLO FYN"). // Override the Global configuration's MY_ENV_VAR
        SetEnvVar("LOCAL_VAR", "HELLO ODENSE").
        AddStep("Maven package").
            AsShell().
                Execute("mvn package").
                Execute("echo \"Value of overridden global env var: \" $MY_ENV_VAR").
                Execute("echo \"Value of local env var: \" $LOCAL_VAR").
                InDirectory(workDir).
    AddJob("install").
        SetName("Install").
        DependsOn("package").
        AddStep("Maven install").
            AsShell().
                Execute("mvn install").
                InDirectory(workDir);

builder.Build();
pipeline = builder.Collect(); // Get the model instance
```

### Generating the YAML output
The model can then be used to generate code as shown below:

```C#
PipelineCodeGen codeGen = new PipelineCodeGen(pipeline); // Use the meta model instance
string YAML = codeGen.Generate();

// It is possible to customize the indentation of the YAML output
codeGen.Indent = 4;
YAML = codeGen.Generate();
```

The output is shown in YAML below with default 2 indentations followed by an example of 4 indentations:

```YAML
name: myPipeline

on: [push]

jobs:

  compile:                                                  # Individual job
    name: Compile
    runs-on: [ubuntu-latest]                                # Global OS
    env: 
      MY_ENV_VAR: "HELLO WORLD!"                            # Global variable
    steps:
      - uses: actions/checkout@v2                           # Global Step
        name: Default Checkout step
      - name: Default clean step                            # Global Step
        working-directory: ./TANKS
        run: |
          echo "My Global Job"
          echo "Value of global env var: " $MY_ENV_VAR
          mvn clean
      - name: Compile Step                                  # Job's step
        working-directory: ./TANKS
        run: mvn compile
  unit-test:                                                # Individual job
    name: Unit test
    runs-on: [ubuntu-16.04]                                 # Override Global OS
    needs: [compile]
    env: 
      MY_ENV_VAR: "HELLO WORLD!"                            # Global variable
    steps:
      - uses: actions/checkout@v2                           # Global step
        name: Default Checkout step
      - name: Default clean step                            # Global step
        working-directory: ./TANKS
        run: |
          echo "My Global Job"
          echo "Value of global env var: " $MY_ENV_VAR
          mvn clean
      - name: Unit Test Step                                # Job's step
        working-directory: ./TANKS
        run: mvn verify
  package:
    name: Package
    runs-on: [ubuntu-18.04]                                 # Override Global OS
    needs: [unit-test]
    env: 
      MY_ENV_VAR: "HELLO FYN"                               # Override Global variable
      LOCAL_VAR: "HELLO ODENSE"                             # Job's variable
    steps:
      - uses: actions/checkout@v2                           # Global step
        name: Default Checkout step
      - name: Default clean step                            # Global step
        working-directory: ./TANKS
        run: |
          echo "My Global Job"
          echo "Value of global env var: " $MY_ENV_VAR
          mvn clean
      - name: Maven package                                 # Job's step
        working-directory: ./TANKS
        run: |
          mvn package
          echo "Value of overridden global env var: " $MY_ENV_VAR
          echo "Value of local env var: " $LOCAL_VAR
  install:
    name: Install
    runs-on: [ubuntu-latest]                                # Global OS
    needs: [package]
    env: 
      MY_ENV_VAR: "HELLO WORLD!"                            # Global variable
    steps:
      - uses: actions/checkout@v2                           # Global step
        name: Default Checkout step
      - name: Default clean step                            # Global step
        working-directory: ./TANKS
        run: |
          echo "My Global Job"
          echo "Value of global env var: " $MY_ENV_VAR
          mvn clean
      - name: Maven install                                 # Job's step
        working-directory: ./TANKS
        run: mvn install
```

Same meta model instance with 4 space indentations:

```YAML
name: myPipeline

on: [push]

jobs:

    compile:
        name: Compile
        runs-on: [ubuntu-latest]
        env: 
            MY_ENV_VAR: "HELLO WORLD!"
        steps:
            - uses: actions/checkout@v2
              name: Default Checkout step
            - name: Default clean step
              working-directory: ./TANKS
              run: |
                  echo "My Global Job"
                  echo "Value of global env var: " $MY_ENV_VAR
                  mvn clean
            - name: Compile Step
              working-directory: ./TANKS
              run: mvn compile
    unit-test:
        name: Unit test
        runs-on: [ubuntu-16.04]
        needs: [compile]
        env: 
            MY_ENV_VAR: "HELLO WORLD!"
        steps:
            - uses: actions/checkout@v2
              name: Default Checkout step
            - name: Default clean step
              working-directory: ./TANKS
              run: |
                  echo "My Global Job"
                  echo "Value of global env var: " $MY_ENV_VAR
                  mvn clean
            - name: Unit Test Step
              working-directory: ./TANKS
              run: mvn verify
    package:
        name: Package
        runs-on: [ubuntu-18.04]
        needs: [unit-test]
        env: 
            MY_ENV_VAR: "HELLO FYN"
            LOCAL_VAR: "HELLO ODENSE"
        steps:
            - uses: actions/checkout@v2
              name: Default Checkout step
            - name: Default clean step
              working-directory: ./TANKS
              run: |
                  echo "My Global Job"
                  echo "Value of global env var: " $MY_ENV_VAR
                  mvn clean
            - name: Maven package
              working-directory: ./TANKS
              run: |
                  mvn package
                  echo "Value of overridden global env var: " $MY_ENV_VAR
                  echo "Value of local env var: " $LOCAL_VAR
    install:
        name: Install
        runs-on: [ubuntu-latest]
        needs: [package]
        env: 
            MY_ENV_VAR: "HELLO WORLD!"
        steps:
            - uses: actions/checkout@v2
              name: Default Checkout step
            - name: Default clean step
              working-directory: ./TANKS
              run: |
                  echo "My Global Job"
                  echo "Value of global env var: " $MY_ENV_VAR
                  mvn clean
            - name: Maven install
              working-directory: ./TANKS
              run: mvn install

```

## Example 2
This example is simplified and minimized. It contains a global configuration but does not make use of overriding and dependencies.

The pipeline looks like this: Compile -> Unit test

### Generating the meta model instance

```C#
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
pipeline = builder.Collect(); // Creates the meta model instance
```

### Generating the YAML output

```C#
PipelineCodeGen codeGen = new PipelineCodeGen(_pipeline);
string YAML = codeGen.Generate();
```


YAML output:
```YAML
name: myPipeline

on: [push]

jobs:

  compile:
    name: efc646e7-a619-4991-9752-53b61ba96eea
    runs-on: [ubuntu-latest]
    env: 
      MY_ENV_VAR: "HELLO WORLD!"
    steps:
      - uses: actions/checkout@v2
        name: Default Checkout step
      - name: Default clean step
        working-directory: ./TANKS
        run: |
          echo "My Global Job"
          mvn clean
      - name: Compile Step
        working-directory: ./TANKS
        run: mvn compile
  unittest:
    name: 0c88aeb2-6426-4fcf-acd3-15c098b36907
    runs-on: [ubuntu-latest]
    env: 
      MY_ENV_VAR: "HELLO WORLD!"
    steps:
      - uses: actions/checkout@v2
        name: Default Checkout step
      - name: Default clean step
        working-directory: ./TANKS
        run: |
          echo "My Global Job"
          mvn clean
      - name: Unit Test Step
        working-directory: ./TANKS
        run: mvn verify
```

# Metal model class diagram UML
![alt text](github-action-metamodel.png)
