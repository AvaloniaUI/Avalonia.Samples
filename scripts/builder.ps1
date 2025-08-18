param (
    [string]$Path = "../src"
)

# Check if 'dotnet' CLI is available
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "'dotnet' command is not found. Please ensure .NET SDK is installed and dotnet is in your PATH."
    Write-Host "You can download it from https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

# Resolve and validate the path
$FullPath = Resolve-Path -Path $Path -ErrorAction SilentlyContinue
if (-not $FullPath) {
    Write-Error "The path '$Path' does not exist."
    Write-Host "Please provide a valid path." -ForegroundColor Yellow
    exit 1
}
$SearchPath = $FullPath.ToString()

Write-Host "Building .NET Core projects under: $SearchPath" -ForegroundColor Cyan

# Find and build all .csproj files
$projects = Get-ChildItem -Path $SearchPath -Filter *.csproj -Recurse

if ($projects.Count -eq 0) {
    Write-Warning "No .csproj files found in '$SearchPath'."
    Write-Host "Please ensure you are in the correct directory." -ForegroundColor Yellow
    exit 0
}

foreach ($proj in $projects) {
    Write-Host "`nBuilding project: $($proj.FullName)" -ForegroundColor Yellow
    $result = & dotnet build $proj.FullName

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed for: $($proj.FullName)"
        Write-Host "Please check the project for errors." -ForegroundColor Yellow
        exit $LASTEXITCODE
    }
}

Write-Host "`nAll projects built successfully." -ForegroundColor Green