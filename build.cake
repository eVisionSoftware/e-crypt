#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=ILRepack"
#addin nuget:?package=Cake.Incubator

using Cake.Common.Tools.GitVersion;
using IoPath = System.IO.Path;
using Cake.Incubator;

var target                  = Argument("target", "Default");
var configuration           = Argument("configuration", "Release");

string buildDir             = Directory("./src/eCrypt/bin") + Directory(configuration);
string keyGeneratorBuildDir = Directory("./src/eCrypt.KeyGenerator/bin") + Directory(configuration);
string cakeAddinBuildDir    = Directory("./src/Cake.eCrypt/bin") + Directory(configuration);
string outputDir            = Directory("./build_output/artifacts");
string solutionPath         = File("./src/eCrypt.sln");

string keyGeneratorExeName  = "eVision.KeyGenerator.exe";
string toolExeName          = "eVision.eCrypt.exe";
string cakeAddinName        = "Cake.eCrypt.dll";

string keyGenArtifactsPath  = Directory(outputDir) + File(keyGeneratorExeName);
string toolAtrifactsPath    = Directory(outputDir) + File(toolExeName);
string cakeAddinArtifactsPath    = Directory(outputDir) + File(cakeAddinName);

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(keyGeneratorBuildDir);
    CleanDirectory(outputDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solutionPath);
});

Task("Update-Assembly-Info")
    .Does(() =>
{
	if (!BuildSystem.IsLocalBuild) 
    {
        GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = true,
            OutputType = GitVersionOutput.BuildServer
        });
    }
	
    GitVersion version = GitVersion(new GitVersionSettings { UpdateAssemblyInfo = false, OutputType = GitVersionOutput.Json });
	Information("Version: " + version.NuGetVersion);
});

Task("Build")
    .IsDependentOn("Update-Assembly-Info")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild(solutionPath, settings => settings.SetConfiguration(configuration));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var testAssemblies = GetFiles("./**/bin/Release/*.Tests.dll");
    XUnit2(testAssemblies);
});

Task("Merge-Libraries")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    string primaryAssembly = Directory(keyGeneratorBuildDir) + File(keyGeneratorExeName);
    var assemblyPaths = new string[] {
        "BouncyCastle.*",
        "CommandLine",
        "Xceed.Wpf.Toolkit",
        "GalaSoft.*",
        "MahApps.Metro",
        "Microsoft.Practices.ServiceLocation",
        "Microsoft.WindowsAPICodePack",
        "Newtonsoft.Json",
        "NLog",
        "Prism",
        "Ookii.Dialogs.Wpf",
        //"Xceed.*", These dlls are embedded as resources
        "System.*"
    }.Select(path => string.Format("{0}/{1}.dll", keyGeneratorBuildDir, path))
     .ToArray();

    CopyFile(Directory(buildDir) + File(toolExeName), toolAtrifactsPath);
    CopyFile(Directory(cakeAddinBuildDir) + File(cakeAddinName), cakeAddinArtifactsPath);

    ILRepack(keyGenArtifactsPath, primaryAssembly, GetFiles(assemblyPaths), new ILRepackSettings(){
        Internalize = true,
        Wildcards = true,
        AllowDuplicateResources = true,
        TargetPlatform = TargetPlatformVersion.v4,
        TargetKind = TargetKind.WinExe
    });
});

Task("Create-Nuget-Packages")
    .IsDependentOn("Merge-Libraries")
    .Does(() =>
{
     string nugetTarget = "lib/net45";
     NuGetPack(GetFiles("./**/eVision.eCrypt.nuspec"), new NuGetPackSettings {
            Version           = GitVersion(new GitVersionSettings()).AssemblySemVer,
            NoPackageAnalysis = true,
            BasePath          = ".",
            OutputDirectory   = outputDir,
            Files             = new [] {
                                  new NuSpecContent {Source = keyGenArtifactsPath, Target = nugetTarget },
                                  new NuSpecContent {Source = toolAtrifactsPath, Target = nugetTarget }
                                }
        });
        
     NuGetPack(GetFiles("./**/Cake.eCrypt.nuspec"), new NuGetPackSettings {
            Version           = GitVersion(new GitVersionSettings()).AssemblySemVer,
            NoPackageAnalysis = true,
            BasePath          = ".",
            OutputDirectory   = outputDir,
            Files             = new [] {
                                  new NuSpecContent {Source = toolAtrifactsPath, Target = nugetTarget },
                                  new NuSpecContent {Source = cakeAddinArtifactsPath, Target = nugetTarget }
                                }
        });
});

Task("Push-Nuget-Packages")
    .IsDependentOn("Create-Nuget-Packages")
    .Does(() =>
{
    string semVersion = GitVersion(new GitVersionSettings()).SemVer;
    var pushSettings = new NuGetPushSettings {
        Source = "https://evision.myget.org/F/main/api/v3/index.json",
        ApiKey = EnvironmentVariable("NugetApiKey")
    };
    
    string package = Directory(outputDir) + File("eVision.eCrypt." + semVersion + ".nupkg");
    Information("Publishing nuget package " + package);
    NuGetPush(package, pushSettings);
    
    package = Directory(outputDir) + File("Cake.eCrypt." + semVersion + ".nupkg");    
    Information("Publishing nuget package " + package);
    NuGetPush(package, pushSettings);
});

Task("Default").IsDependentOn("Run-Unit-Tests");
Task("Package").IsDependentOn("Create-Nuget-Packages");
Task("Publish").IsDependentOn("Push-Nuget-Packages");
RunTarget(target);