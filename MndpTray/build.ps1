#-----------------------------------------------------------------------------
# * Project:    MndpTray
# * Repository: https://github.com/xmegz/MndpTray
# * Author:     Pádár Tamás
# * File:       build.ps1
# ----------------------------------------------------------------------------

#
# To enable script execution, run at admin powershell prompt
# 'set-executionpolicy remotesigned'
#

#
# Configuration
#

$SolutionFolderPath=$PSScriptRoot
$BuildFolderPath="$SolutionFolderPath\Build"

Write-Host ""
Write-Host "Script: [$PSCommandPath]" -ForegroundColor Blue
Write-Host ""
Write-Host "Config:"
Write-Host "SolutionFolderPath: $SolutionFolderPath";
Write-Host "BuildFolderPath: $BuildFolderPath";
Write-Host ""
Write-Host ""

#
# Clean or Create Output Folder
#

if (Test-Path -LiteralPath $BuildFolderPath) 
{
    Remove-Item -LiteralPath $BuildFolderPath -Verbose -Recurse -WhatIf
}
else
{
	New-Item -ItemType Directory -Path $BuildFolderPath -Verbose -Force
}

#
# Build & Publish & Copy Libs
#
$ProjectsLibs = @(    
    'MndpTray.Protocol'    
)

foreach ( $Project in $ProjectsLibs )
{
    Write-Host ""
    Write-Host "Project: [$Project]" -ForegroundColor Blue
    Write-Host ""
    
    dotnet clean -c Release $SolutionFolderPath\$Project
    dotnet build -c Release $SolutionFolderPath\$Project
    dotnet pack $SolutionFolderPath\$Project -o $BuildFolderPath
}

#
# Build & Publish Console
#


$ProjectsConsole = @(    
    'MndpService.Core'    
)

$ProfilesConsole = @(    
    'win-x64'    
    'win-x64-full'    
    'linux-x64'    
    'linux-x64-full'    
)


foreach ( $Project in $ProjectsConsole )
{
    Write-Host ""
    Write-Host "Project: [$Project]" -ForegroundColor Blue
    Write-Host ""

    dotnet clean -c Release $SolutionFolderPath\$Project
    dotnet build -c Release $SolutionFolderPath\$Project

    foreach ( $Prof in $ProfilesConsole )
    {
        Write-Host ""
        Write-Host "Profile: [$Prof]"  -ForegroundColor Blue
        Write-Host ""
        
        dotnet publish -c Release $SolutionFolderPath\$Project /p:PublishProfile=$Prof
    }
}


#
# Copy output
#
foreach ( $Project in $ProjectsConsole )
{
    Write-Host ""
    Write-Host "Copy: [$Project]"  -ForegroundColor Blue
    Write-Host ""

    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0\publish\win-x64\$Project.exe -Destination $BuildFolderPath\$Project.exe
    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0\publish\win-x64-full\$Project.exe -Destination $BuildFolderPath\$Project.Full.exe
    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0\publish\linux-x64\$Project -Destination $BuildFolderPath\$Project
    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0\publish\linux-x64-full\$Project -Destination $BuildFolderPath\$Project.Full
}


#
# Build & Publish Windows
#

$ProjectsWindows = @(    
    'MndpTray.Core'    
)

$ProfilesWindows = @(    
    'win-x64'    
    'win-x64-full'        
)


foreach ( $Project in $ProjectsWindows )
{
    Write-Host ""
    Write-Host "Project: [$Project]" -ForegroundColor Blue
    Write-Host ""
    
    dotnet clean -c Release $SolutionFolderPath\$Project
    dotnet build -c Release $SolutionFolderPath\$Project

    foreach ( $Prof in $ProfilesWindows )
    {
        Write-Host ""
        Write-Host "Profile: [$Prof]" -ForegroundColor Blue
        Write-Host ""
        
        dotnet publish -c Release $SolutionFolderPath\$Project /p:PublishProfile=$Prof
    }
}

#
# Copy output
#
foreach ( $Project in $ProjectsWindows )
{
    Write-Host ""
    Write-Host "Copy: [$Project]" -ForegroundColor Blue
    Write-Host ""

    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0-windows\publish\win-x64\$Project.exe -Destination $BuildFolderPath\$Project.exe
    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0-windows\publish\win-x64-full\$Project.exe -Destination $BuildFolderPath\$Project.Full.exe
}
