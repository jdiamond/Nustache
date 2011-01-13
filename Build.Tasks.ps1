$msbuild   = "$(get-content env:systemroot)\Microsoft.NET\Framework\v3.5\MSBuild.exe /tv:3.5"

task Clean {
    Log-Message "Cleaning the project..."

    Get-ChildItem $rootDir -Include bin, obj -Recurse | ? { -not $_.FullName.Contains("Lib") } | %{ Remove-Item $_\* -Force -Recurse }
    Remove-Item $buildDir -Force -Recurse -ErrorAction silentlycontinue

    iex "$msbuild $solution /t:clean $verbose"

    if (-not (Test-Path $buildDir)) {
        $output = mkdir $buildDir
    }
}

task Compile {
    Log-Message "Compiling $solution ..."

    iex "$msbuild $solution /p:Configuration=$buildConfiguration $verbose"
    
    Exit-IfError "Build Failed - Compilation"
}

task RunUnitTests {
    Run-Test "Unit"
}

function Exit-IfError {
    param ($message)

    if ($lastexitcode -ne 0) {
        Write-Host -foreground red $message
        exit 1
    }
}

function Log-Message {
    param ($message, $type = "message")
    
    switch ($type.ToLower()){
        "message" { $messageColor = "Green" }
        "error"   { $messageColor = "Red" }
        "warning" { $messageColor = "Yellow" }
        "info"    { $messageColor = "Cyan" }
    }
    
    Write-Host -ForegroundColor $messageColor "`n==========================================="
    Write-Host -ForegroundColor $messageColor $message
    Write-Host -ForegroundColor $messageColor "===========================================`n"
}

function Get-TestAssemblies() {
    $testProjects = ""

    Get-ChildItem "$rootDir\*.Tests\*.csproj" | %{
        $testProjects += "$(split-path $_.fullname)\bin\$buildConfiguration\$([system.io.path]::GetFilenameWithoutExtension($_.name)).dll "
    }
    return $testProjects
}

function Run-Test($testType) {
    $nunit = "$rootDir\Lib\NUnit\nunit-console.exe"
    $testProjects = Get-TestAssemblies

    Log-Message "Running $testType Tests..."
    iex "$nunit $testProjects /xml=$buildDir\$testType-Test-Reports.xml"

    Exit-IfError "Build Failed - $testType Test".toUpper()
}

