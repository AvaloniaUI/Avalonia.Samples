param (
    [string]$Path = "../src"
)

Clear-Host
# Resolve and validate path
$FullPath = Resolve-Path -Path $Path -ErrorAction SilentlyContinue
if (-not $FullPath) {
    Write-Error "The path '$Path' does not exist."
    Write-Host "Please provide a valid path." -ForegroundColor Yellow
    exit 1
}
$SearchPath = $FullPath.ToString()

# Check if .NET 9 is installed
$dotnetSdks = & dotnet --list-sdks
if ($dotnetSdks -notmatch "^9\.\d+\.\d+") {
    Write-Error ".NET 9 SDK is not installed. Please install it before running this script."
    Write-Host "You can download it from https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor Yellow
    Write-Host "Exiting script." -ForegroundColor Yellow
    exit 1
}

Write-Host "Targeting path: $SearchPath" -ForegroundColor Cyan
Write-Host "Searching for .csproj files to retarget to .NET 9..." -ForegroundColor Cyan

# Find and update all .csproj files
Get-ChildItem -Path $SearchPath -Filter *.csproj -Recurse | ForEach-Object {
    $projFile = $_.FullName
    Write-Host "Processing: $projFile" -ForegroundColor Yellow

    $content = Get-Content $projFile
    $updated = $false

    $newContent = $content | ForEach-Object {
        if ($_ -match "<TargetFramework>(.*)</TargetFramework>") {
            $oldTarget = $matches[1]
            if ($oldTarget -ne "net9.0") {
                $_ = $_ -replace "<TargetFramework>.*</TargetFramework>", "<TargetFramework>net9.0</TargetFramework>"
                $updated = $true
            }
        }
        return $_
    }

    if ($updated) {
        $newContent | Set-Content $projFile
        Write-Host "Updated target framework to net9.0" -ForegroundColor Green
    } else {
        Write-Host "No changes needed." -ForegroundColor Yellow
    }
}

Write-Host "Retargeting completed." -ForegroundColor Green