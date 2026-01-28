#-----------------------------------------------------------------------------
# * Project:    MndpTray
# * Repository: https://github.com/xmegz/MndpTray
# * Author:     Pádár Tamás
# ----------------------------------------------------------------------------

#
# Switch to admin user
#
$CurrentUser = New-Object Security.Principal.WindowsPrincipal $([Security.Principal.WindowsIdentity]::GetCurrent())
if ($CurrentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator) -eq $false) {
    Start-Process powershell.exe -Verb RunAs -ArgumentList ('-noprofile -noexit -file "{0}" -elevated' -f ($myinvocation.MyCommand.Definition))
    exit $LASTEXITCODE
}

#
# Configuration
#
$ProgramPath    = "C:\Program Files\MndpService"
$ProgramName    = "MndpService.Core.Full.exe"
$ServiceName    = "MndpService.Core"
$ServiceDesc    = "MndpService is a background service, which is send information about running host"
$ServiceVersion = "2.3.0"

$DownloadUrl    = "https://github.com/xmegz/MndpTray/releases/download/v$ServiceVersion/MndpService.Core.Full.exe"
$LogFile        = "$ProgramPath\install.txt"
$ProgramFile    = "$ProgramPath\$ProgramName"

#
# Logging function
#
function Write-Log {
    param([string]$message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $line = "$timestamp - $message"
    Write-Host $line
    Add-Content -Path $logFile -Value $line
}

#
# Create installation folder
#
md -Force $ProgramPath | Out-Null

#
# Remove existing service if it exists
#
if (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) {
    Write-Log "Service is running, stopping it..."
    
    try {
        Write-Log "Stopping service..."
        Stop-Service -Name $ServiceName -Force -ErrorAction Stop
    } catch {
        Write-Log "Service could not be stopped or was already stopped"
    }

    Write-Log "Deleting service..."
    sc.exe delete $ServiceName | Out-Null
    Start-Sleep -Seconds 2
}

#
# Download the executable
#
Write-Log "Downloading executable from $DownloadUrl"
$ProgressPreference = 'SilentlyContinue'
Invoke-WebRequest -Uri $DownloadUrl -OutFile "$ProgramFile"

#
# Check if download was successful
#
if (-Not (Test-Path "$ProgramFile")) {
    Write-Log "Download failed. Exiting."
    exit 1
}

#
# Create the Windows service
#
Write-Log "Creating Windows service: $ServiceName"
sc.exe create $ServiceName binPath= "`"$ProgramFile`"" start= auto DisplayName= "$ServiceName" | Out-Null
sc.exe description $ServiceName $ServiceDesc

#
# Start the service
#
Write-Log "Starting service..."
Start-Service -Name $ServiceName

#
# Check if service is running
#
$svc = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($svc.Status -eq "Running") {
    Write-Log "Service is running: $ServiceName"
} else {
    Write-Log "Service is not running after installation"
}
