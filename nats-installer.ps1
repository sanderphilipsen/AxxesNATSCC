Write-Host "Welcome to the installation of NATS" -ForegroundColor Green

# Check if the script is executed in administrator mode
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
$isAdministrator = $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if($false -eq $isAdministrator){
    Write-Host "The script is not running in administrator mode" -ForegroundColor Red
    Exit
}

## Choose the deploy folder for all the NATS dependencies
$deployFolder = Read-Host -Prompt "Choose the deploy folder"

if("" -eq $deployFolder){
    $deployFolder = "."
}

$deployFolder = (( Resolve-Path $deployFolder).Path).TrimEnd("\")
Write-Host "Deploy folder is '$deployFolder'" -ForegroundColor Green

Write-Host "`r`nInstalling and configuring nats environment" -ForegroundColor Cyan

## Stop the NATS-server service if it exists
$natsService = Get-Service -Name nats-server -ErrorAction Ignore
if ($null -ne $natsService) {
    Write-Host -ForegroundColor Yellow "Nats-server service was already installed on system. Upgrading with new configuration"
    try {
        Stop-Service -Name nats-server
        $natsService.WaitForStatus("Stopped", "00:00:05");
        . sc.exe delete nats-server >> $logFile
        Write-Host -ForegroundColor Green "Successfully removed nats-server windows service"
    }
    catch {
        Write-Host -ForegroundColor Red "There was an issue when removing the nats-server windows service"
        Exit
    }
}

## Download the NATS-server
$latestRelease = Invoke-WebRequest https://github.com/nats-io/nats-server/releases/latest -Headers @{"Accept"="application/json"}
$json = $latestRelease.Content | ConvertFrom-Json
$latestVersion = $json.tag_name
$fileName = "nats-server-$latestVersion-windows-amd64"

$uri = "https://github.com/nats-io/nats-server/releases/download/$latestVersion/$fileName.zip"

# Create the directories for the NATS-server file
New-Item -ItemType Directory -Force -Path $deployFolder\nats-server

$outFile = "$deployFolder\$fileName.zip"

Write-Host "Downloading NATS server $latestVersion" -ForegroundColor DarkYellow

Invoke-WebRequest -Uri $uri -OutFile $outFile
Expand-Archive  "$deployFolder\$fileName.zip" $deployFolder -Force

$natsServerExeDirectory = "$deployFolder\$fileName"
$natsServer = $natsServerExeDirectory.Replace("\", "/").TrimEnd("/")+"/nats-server.exe"

# Move the NATS-server exe
$natsServer = Get-ChildItem -Path $natsServer

Move-Item $natsServer "$deployFolder/nats-server/nats-server.exe" -Force

Remove-Item $outFile -Force
Remove-Item $natsServerExeDirectory -Recurse

$natsServerDirectory = ("$deployFolder/nats-server").Replace("/", "\")

Write-Host "Nats server installed" -ForegroundColor Green