param (
    [string]$Path = "../src"
)

Clear-Host
Write-Host "Deleting all BIN and OBJ folders..." -ForegroundColor Cyan

# Resolve the full path
$FullPath = Resolve-Path -Path $Path -ErrorAction SilentlyContinue

# Check if the path exists
if (-not $FullPath) {
    Write-Error "The path '$Path' does not exist."
    Write-Host "Please provide a valid path." -ForegroundColor Yellow
    exit 1
}

# Convert to string in case Resolve-Path returns a PathInfo object
$SearchPath = $FullPath.ToString()

Write-Host "Searching for 'bin' and 'obj' directories under '$SearchPath'..." -ForegroundColor Cyan

# Find and remove 'bin' and 'obj' folders recursively
Get-ChildItem -Path $SearchPath -Include bin,obj -Directory -Recurse -Force |
    ForEach-Object {
        Write-Host "Removing folder: $($_.FullName)" -ForegroundColor Yellow
        Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "Successfully removed folder: $($_.FullName)" -ForegroundColor Green
    }

Write-Host "Cleanup completed." -ForegroundColor Green