Param(
    [string] $Configuration='Release',
    [string] $VersionPrefix='1.17.0',
    [string] $VersionSuffix='alpha1'
)

$dotnet = Get-Command 'dotnet'

& $dotnet restore /p:VersionPrefix=$VersionPrefix /p:VersionSuffix=$VersionSuffix
& $dotnet pack /p:Configuration=Release /p:VersionPrefix=$VersionPrefix /p:VersionSuffix=$VersionSuffix -o $PWD\out
