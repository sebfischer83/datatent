using System;
using System.Linq;
using System.Runtime.InteropServices;
using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.CoverallsNet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
//[AzurePipelines(AzurePipelinesImage.WindowsLatest,  InvokedTargets = new []{ nameof(PublishCoverage) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.PublishCoverage);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Explicit framework to build")] readonly string Framework = null;
    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetFramework(Framework)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution.GetProject("Datatent.Core.Tests"))
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetLogger("trx")
                .SetLogOutput(true)
                .SetArgumentConfigurator(arguments => arguments.Add("/p:CollectCoverage={0}", true)
                    .Add("/p:CoverletOutput={0}/", ArtifactsDirectory / "coverage")
                    .Add("/p:UseSourceLink={0}", "true")
                    .Add("/p:CoverletOutputFormat={0}", "cobertura")
                    .Add("/p:CoverletOutputFormat={0}", "opencover"))
                .SetResultsDirectory(ArtifactsDirectory / "tests"));
        });


    Target PublishCoverage => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {

            if (System.Runtime.InteropServices.RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows) && !IsLocalBuild)
            {
                DotNetToolUpdate(settings => settings
                    .SetGlobal(true)
                    .SetPackageName("Codecov.Tool"));
                ProcessTasks.StartProcess("codecov.exe", 
                    arguments: $"-f \"{ArtifactsDirectory / "coverage/coverage.opencover.xml"}\" -t 8532ee50-6d63-4a1a-a36f-ad2741eb3e40");
            }
        });
}
