param(
    $task = @("Clean", "Compile", "RunUnitTests", "Package"),
    $verbose = "/noconsolelogger"  #q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]. e.g.: /v:n
)

$rootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Import-Module $rootDir\Lib\PSake\psake.psm1 -force

Invoke-Psake .\Build.Tasks.ps1 -taskList $task `
    -parameters @{
        "rootDir" = $rootDir;
        "verbose" = $verbose;
        "buildDir" = "$rootDir\build";
        "packageDir" = "$rootDir\build\package";
        "solution" = (Resolve-Path *.sln);
        "buildConfiguration" = "Release"
      }

