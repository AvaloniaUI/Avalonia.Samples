# Compiles each .adoc file in all subdirectories (recursively) into a HTML-file using Asciidoctor-CLI.
# If specific files are provided as arguments, only those files will be processed.

param(
    [Parameter(Mandatory=$false, ValueFromRemainingArguments=$true)]
    [string[]]$InputFiles
)

# Determine repository root in a robust way:
# - On GitHub Actions runners GITHUB_WORKSPACE is set to the repository checkout root.
# - When running locally, derive the repo root from the script's folder (scripts/) by taking the parent.
$repoRoot = if ($env:GITHUB_WORKSPACE) {
    $env:GITHUB_WORKSPACE
} elseif ($PSScriptRoot) {
    # script is in scripts/, repository root is its parent
    Split-Path -Path $PSScriptRoot -Parent
} else {
    (Get-Location).Path
}

Write-Output "Repository root resolved to: $repoRoot"

$docsDir = Join-Path -Path $repoRoot -ChildPath '_docs'
$srcDir  = Join-Path -Path $repoRoot -ChildPath 'src'

Write-Output "Looking for docs in: $docsDir"
Write-Output "Looking for source in: $srcDir"

# Resolve theme and favicon paths (fail early with clear messages)
$themeFile = Join-Path -Path $docsDir -ChildPath 'avalonia-docs-theme.css'
$favFile   = Join-Path -Path $docsDir -ChildPath '_Assets/Logo.svg'

if (-not (Test-Path -Path $themeFile)) {
    Write-Error "Theme file not found at: $themeFile"
    exit 1
}
if (-not (Test-Path -Path $favFile)) {
    Write-Error "Favicon file not found at: $favFile"
    exit 1
}

$themePathObj = Resolve-Path -Path $themeFile
$favIconPathObj = Resolve-Path -Path $favFile

Write-Output "Resolved theme path: $($themePathObj.Path)"
Write-Output "Resolved favicon path: $($favIconPathObj)"

if ($InputFiles -and $InputFiles.Count -gt 0) {
    $adocFiles = $InputFiles | ForEach-Object {
        try { 
            Get-Item $_ 
        } catch { 
            Write-Warning "File not found: $_"
            $null 
        }
    } | Where-Object { $_ -ne $null }
} else {
    # Find .adoc files recursively in src
    if (Test-Path -Path $srcDir) {
        $adocFiles = Get-ChildItem -Path $srcDir -Include *.adoc -Recurse -File 
    } else {
        $adocFiles = @()
    }

    # Add the root README.adoc if it exists
    $readmePath = Join-Path -Path $repoRoot -ChildPath 'README.adoc'
    if (Test-Path -Path $readmePath) {
        $adocFiles += Get-Item -Path $readmePath
    }
}

if (-not $adocFiles -or $adocFiles.Count -eq 0) {
    Write-Error "No AsciiDoc files found. Exiting."
    exit 0
}


foreach ($file in $adocFiles) {
    try {

        $filePath = $file.FullName
        # Build attribute list as an argument array so each token is passed separately
        $cmdArgs = @($filePath)

        $cmdArgs += "-a"; $cmdArgs += "source-highlighter=rouge"

        # $cmdArgs += "-a"; $cmdArgs += "imagesdir=`"`" "

        $themePath = ($themePathObj.Path) -replace '\\', '/'
        $cmdArgs += "-a"; $cmdArgs += "stylesheet=$themePath"

        $favPath = (Resolve-Path -Path $favFile -Relative -RelativeBasePath $file.Directory) -replace '\\', '/'
        $cmdArgs += "-a"; $cmdArgs += "favicon=$favPath"

        # Any ReadMe.adoc should create an index.html file. This will help to create a github.io page
        if ($file.Name -ieq "ReadMe.adoc") {
            $outFileName = Join-Path -Path $file.Directory -ChildPath 'index.html'
            $cmdArgs += "-o"; $cmdArgs += $outFileName
        }

        # enable trace logging
        $cmdArgs += "--trace"

        Write-Output "Asciidoctor attributes: $($cmdArgs -join ' ')"
        Write-Output "Processing $filePath"

        # Call asciidoctor with argument array so arguments are preserved
        & asciidoctor @cmdArgs

    } catch {
        Write-Error "Failed to process $($file.FullName): $_"
    }
}
