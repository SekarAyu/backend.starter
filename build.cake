#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin nuget:?package=Cake.FileHelpers&version=4.0.0

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var projectBasePath = Argument("path", ".");
var repo = Argument("repo", "qsirepo");
var preReleaseRepo = Argument("preReleaseRepo", "qsirepodev");

var envFile = ".env.secret";
var envVariables = new Dictionary<string, string>();
if (FileExists(envFile))
{
    var envs = System.IO.File.ReadAllLines(envFile);
    foreach (var env in envs)
    {
        var _env = env.Split('=');
        envVariables.Add(_env[0], _env[1]);
    }
}

var ignorePath = ".nugetignore";
var ignoredProjectsPath = new List<string>() {
    "(.*?).Test.csproj"
};
if (FileExists(ignorePath))
{
    ignoredProjectsPath.AddRange(System.IO.File.ReadAllLines(ignorePath));
}

Func<IFile, bool> NotIngoredProjects = file =>
{
    var fPath = file.Path.FullPath;
    return ignoredProjectsPath.Select(x => new Regex(x).Match(fPath).Success).Where(x => x).Count() <= 0;
};

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////



//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    Information(projectBasePath);
    var projects = GetFiles(projectBasePath + "/**/*.csproj");

    Information("Found " + projects.Count + " projects in: " + projectBasePath);
    foreach (var project in projects)
    {
        Information($"Cleaning {project.GetFilenameWithoutExtension()}");
        DotNetCoreClean(project.FullPath);
    }

});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var projects = GetFiles(projectBasePath + "/**/*.csproj", new GlobberSettings { FilePredicate = NotIngoredProjects });
    // Write to output (build log)
    Information("Found " + projects.Count + " projects in: " + projectBasePath);
    foreach (var project in projects)
    {
        Information($"Building {project.GetFilenameWithoutExtension()}");
        var pr = FileReadText(project);
        var r = Regex.Match(pr, "<TargetFrameworks>(.*?)</TargetFrameworks>", RegexOptions.IgnoreCase);
        if (r.Success)
        {
            // multiple target framework project
            var frameworks = r.Groups[1].Value.Split(';');
            foreach (var framework in frameworks)
            {
                Information($"Building Target Framework {framework}");
                DotNetCoreBuild(project.FullPath, new DotNetCoreBuildSettings
                {
                    Framework = framework,
                    Configuration = "Release",
                    EnvironmentVariables = envVariables
                });
            }
        }
        else
        {
            // not a multiple target framework project
            DotNetCoreBuild(project.FullPath, new DotNetCoreBuildSettings
            {
                Configuration = "Release",
                EnvironmentVariables = envVariables
            });
        }
    }
});

Task("Test")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        WorkingDirectory = Directory(projectBasePath),
        NoBuild = true,
        EnvironmentVariables = envVariables
    };
    var testProjects = GetDirectories(projectBasePath + "/**/*.Test");
    foreach (var testProject in testProjects)
    {
        Information($"Testing {testProject.GetDirectoryName()}");
        DotNetCoreTest(testProject.FullPath);
    }
});

// Create nuget packages for projects
Task("Pack")
    .Does(() =>
{
    var projects = GetFiles(projectBasePath + "/**/*.csproj", new GlobberSettings { FilePredicate = NotIngoredProjects });

    var settings = new DotNetCorePackSettings()
    {
        Configuration = "Release",
        EnvironmentVariables = envVariables
    };
    foreach (var project in projects)
    {
        Information($"Packing {project.GetFilenameWithoutExtension()}");
        DotNetCorePack(project.FullPath, settings);
    }
});

// Push nuget packages to nugetfeed
Task("Push")
    .IsDependentOn("Pack")
    .Does(() =>
{
    Func<IFile, bool> PushablePackage = file => !file.Path.FullPath.EndsWith(".Test.csproj", StringComparison.OrdinalIgnoreCase)
                                                    && !file.Path.FullPath.EndsWith(".Docker.Linux.csproj", StringComparison.OrdinalIgnoreCase);
    var projects = GetFiles(projectBasePath + "/**/*.csproj", new GlobberSettings { FilePredicate = PushablePackage });

    Information($"Found {projects.Count} projects in: {projectBasePath}");
    var propsFile = "./Directory.build.props";
    var version = XmlPeek(propsFile, "//Version");
    foreach (var project in projects)
    {
        var assemblyInfo = ParseAssemblyInfo(project.FullPath);
        Information($"Finding {project.GetFilenameWithoutExtension()}.{version}.nupkg");
        var nugetFiles = GetFiles(projectBasePath + $"/**/*{project.GetFilenameWithoutExtension()}.{version}.nupkg");

        foreach (var nugetFile in nugetFiles)
        {
            Information($"Pushing {nugetFile.GetFilename()}");
            var isPrerelease = Regex.IsMatch(version, "[a-z-]+", RegexOptions.IgnoreCase); ;
            var settings = new NuGetPushSettings()
            {
                Source = isPrerelease ? preReleaseRepo : repo,
                EnvironmentVariables = envVariables
            };
            NuGetPush(nugetFile.FullPath, settings);
        }
    }
});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .Does(() =>
    {
    });

RunTarget(target);
