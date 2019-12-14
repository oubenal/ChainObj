import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.dotnetBuild
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.dotnetTest
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.exec
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.GitVcsRoot

/*
The settings script is an entry point for defining a TeamCity
project hierarchy. The script should contain a single call to the
project() function with a Project instance or an init function as
an argument.

VcsRoots, BuildTypes, Templates, and subprojects can be
registered inside the project using the vcsRoot(), buildType(),
template(), and subProject() methods respectively.

To debug settings scripts in command-line, run the

    mvnDebug org.jetbrains.teamcity:teamcity-configs-maven-plugin:generate

command and attach your debugger to the port 8000.

To debug in IntelliJ Idea, open the 'Maven Projects' tool window (View
-> Tool Windows -> Maven Projects), find the generate task node
(Plugins -> teamcity-configs -> teamcity-configs:generate), the
'Debug' option is available in the context menu for the task.
*/

version = "2019.2"

project {
    description = "play ground for build config"

    vcsRoot(ChainObjVcs)

    buildType(TestSolution)
    buildType(CodeInspection)
    buildType(Compilation)
    buildType(Report)
}

object CodeInspection : BuildType({
    name = "Code Inspection"
    description = "Run inspection code with Resharper CLT"

    artifactRules = """%RESULT_DIR%\*"""
    buildNumberPattern = "%BuildNumber%"

    params {
        text("env.INSPECTCODE_CACHE", """%teamcity.agent.home.dir%\cache\inspectcode""", label = "CACHE_PATH", description = "Inspect code cache", display = ParameterDisplay.HIDDEN, readOnly = true, allowEmpty = true)
        param("BuildNumber", "${Compilation.depParamRefs.buildNumber}")
        param("RESULT_DIR", """.build\inspection""")
        param("RESULT_PATH", """%RESULT_DIR%\inspections.xml""")
    }

    vcs {
        root(ChainObjVcs)
    }

    steps {
        step {
            name = "Run Inspections"
            type = "dotnet-tools-inspectcode"
            param("dotnet-tools-inspectcode.customCmdArgs", "--output=%RESULT_PATH% --severity=INFO --absolute-paths --swea --verbosity=WARN --no-buildin-settings --caches-home=%env.INSPECTCODE_CACHE%")
            param("dotnet-tools-inspectcode.solution", "ChainObj.sln")
            param("dotnet-tools-inspectcodeCustomSettingsProfile", "All.Proj.DotSettings")
            param("jetbrains.resharper-clt.platform", "x64")
            param("jetbrains.resharper-clt.clt-path", "%teamcity.tool.jetbrains.resharper-clt.DEFAULT%")
        }
        powerShell {
            name = "Import Result"
            scriptMode = script {
                content = """
                    param( 
                    [Parameter(Mandatory=${'$'}true)] [String]${'$'}reportPath)
                    
                    Write-Output "Exporting inspection result from ${'$'}reportPath to TeamCity with Service Messages"
                    Write-Output "##teamcity[disableServiceMessages]"
                    Write-Output "##teamcity[importData type='ReSharperInspectCode' path='${'$'}reportPath']"
                    Write-Output "##teamcity[enableServiceMessages]"
                """.trimIndent()
            }
            param("jetbrains_powershell_scriptArguments", "-reportPath:%RESULT_PATH%")
        }
    }

    triggers {
        finishBuildTrigger {
            buildType = "${Compilation.id}"
            successfulOnly = true
        }
    }

    dependencies {
        snapshot(Compilation) {
            onDependencyFailure = FailureAction.FAIL_TO_START
        }
    }
})

object Compilation : BuildType({
    name = "Compilation"
    description = "Compile solution"

    artifactRules = """_temp\bin\Release_AnyCPU => bin.zip"""
    buildNumberPattern = "%system.teamcity.version%_%build.counter%"

    vcs {
        root(ChainObjVcs)
    }

    steps {
        dotnetBuild {
            name = "Release net472"
            projects = "ChainObj.sln"
            framework = "net472"
            configuration = "Release"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        dotnetBuild {
            name = "Release netcoreapp2.1"
            projects = "ChainObj.sln"
            framework = "netcoreapp2.1"
            configuration = "Release"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        dotnetBuild {
            name = "Debug net472"
            projects = "ChainObj.sln"
            framework = "net472"
            configuration = "Debug"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        dotnetBuild {
            name = "Debug netcoreapp2.1"
            projects = "ChainObj.sln"
            framework = "netcoreapp2.1"
            configuration = "Debug"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
    }
})

object Report : BuildType({
    name = "Report"

    buildNumberPattern = "%BuildNumber%"

    params {
        param("REPORT_PATH", "${CodeInspection.depParamRefs["RESULT_PATH"]}")
        param("BuildNumber", "${CodeInspection.depParamRefs["BuildNumber"]}")
        param("TOOL_DIR", ".bin/ReportTransformer/")
    }

    vcs {
        root(ChainObjVcs)
    }

    steps {
        powerShell {
            scriptMode = script {
                content = """
                    param( 
                    [Parameter(Mandatory=${'$'}true)] [String]${'$'}reportPath)
                    
                    Write-Output "Powershell version: ${'$'}(${'$'}PSVersionTable.PSVersion)"
                    Write-Output "Exporting inspection result from ${'$'}reportPath to TeamCity with Service Messages"
                    
                    Write-Output "##teamcity[importData type='ReSharperInspectCode' path='${'$'}reportPath']"
                """.trimIndent()
            }
            param("jetbrains_powershell_scriptArguments", "-reportPath:%REPORT_PATH%")
        }
        exec {
            name = "Filter report"
            path = """%TOOL_DIR%\ReReportTransformer.exe"""
            arguments = "%REPORT_PATH%"
            param("script.content", "%TOOL_DIR%/ReReportTransformer.exe %REPORT_PATH%")
        }
    }

    dependencies {
        dependency(CodeInspection) {
            snapshot {
                onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "inspections.xml=>${CodeInspection.depParamRefs["RESULT_DIR"]}"
            }
        }
        artifacts(Compilation) {
            artifactRules = "bin.zip!/ReReportTransformer/net472/ => %TOOL_DIR%"
        }
    }
})

object TestSolution : BuildType({
    name = "Test Solution"
    description = "run all unit tests"

    buildNumberPattern = "%BuildNumber%"

    params {
        param("BuildNumber", "${Compilation.depParamRefs.buildNumber}")
    }

    vcs {
        root(ChainObjVcs)

        checkoutMode = CheckoutMode.ON_AGENT
        checkoutDir = "SandBox_ChainObjVcs"
    }

    steps {
        dotnetTest {
            projects = "ChainObj.UnitTests/ChainObj.UnitTests.csproj"
        }
    }

    triggers {
        finishBuildTrigger {
            buildType = "${Compilation.id}"
            successfulOnly = true
        }
    }

    dependencies {
        snapshot(Compilation) {
            onDependencyFailure = FailureAction.FAIL_TO_START
        }
    }
})

object ChainObjVcs : GitVcsRoot({
    name = "ChainObj Vcs"
    pollInterval = 1800
    url = "https://github.com/oubenal/ChainObj.git"
    userNameStyle = GitVcsRoot.UserNameStyle.FULL
})
