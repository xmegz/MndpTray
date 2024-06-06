#
# To enable script execution, run at admin powershell prompt
# 'set-executionpolicy remotesigned'
#

$SolutionFolderPath=$PSScriptRoot
$BuildFolderPath="$SolutionFolderPath\Build"

echo "SolutionFolderPath: $SolutionFolderPath";
echo "BuildFolderPath: $BuildFolderPath";


#
# Remove & Clean Output Folder
#

if (Test-Path -LiteralPath $BuildFolderPath) 
{
    Remove-Item -LiteralPath $BuildFolderPath -Verbose -Recurse -WhatIf
}
md -Force $BuildFolderPath



#
# Publish Console
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


foreach ( $Project in $ProjectsLinux )
{
    echo ""
    echo "Project: [$Project]"
    echo ""
    dotnet clean -c Release $SolutionFolderPath\$Project
    dotnet build -c Release $SolutionFolderPath\$Project

    foreach ( $Profile in $ProfilesConsole )
    {
        echo ""
        echo "Profile: [$Profile]"
        echo ""
        
        dotnet publish -c Release $SolutionFolderPath\$Project /p:PublishProfile=$Profile        
    }
}


#
# Copy output
#
foreach ( $Project in $ProjectsConsole )
{
    echo ""
    echo "Copy: [$Project]"
    echo ""

    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0\publish\win-x64\$Project.exe -Destination $BuildFolderPath\$Project.exe
    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0\publish\win-x64-full\$Project.exe -Destination $BuildFolderPath\$Project.Full.exe
    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0\publish\linux-x64\$Project -Destination $BuildFolderPath\$Project
    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0\publish\linux-x64-full\$Project -Destination $BuildFolderPath\$Project.Full
}


#
# Publish Windows
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
    echo ""
    echo "Project: [$Project]"
    echo ""
    dotnet clean -c Release $SolutionFolderPath\$Project
    dotnet build -c Release $SolutionFolderPath\$Project

    foreach ( $Profile in $ProfilesWindows )
    {
        echo ""
        echo "Profile: [$Profile]"
        echo ""
        
        dotnet publish -c Release $SolutionFolderPath\$Project /p:PublishProfile=$Profile        
    }
}

#
# Copy output
#
foreach ( $Project in $ProjectsWindows )
{
    echo ""
    echo "Copy: [$Project]"
    echo ""

    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0-windows\publish\win-x64\$Project.exe -Destination $BuildFolderPath\$Project.exe
    Copy-Item -Verbose -Path $SolutionFolderPath\$Project\bin\Release\net8.0-windows\publish\win-x64-full\$Project.exe -Destination $BuildFolderPath\$Project.Full.exe
}
