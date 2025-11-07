# Compiles each .adoc file in all subdirectories (recursively) into a HTML-file using Asciidoctor-CLI.
# If specific files are provided as arguments, only those files will be processed.

param(
    [Parameter(Mandatory=$false, ValueFromRemainingArguments=$true)]
    [string[]]$InputFiles
)

# Resolve script directory reliably
$scriptDir = if ($env:GITHUB_WORKSPACE) { $env:GITHUB_WORKSPACE } else { $PSScriptRoot }
$docsDir = Join-Path -Path $scriptDir -ChildPath '../_docs'
$srcDir = Join-Path -Path $scriptDir -ChildPath '../src'

# Resolve theme and favicon paths
try {
    $themePathObj = Resolve-Path -Path (Join-Path -Path $docsDir -ChildPath 'avalonia-docs-theme.css')
    $favIconPathObj = Resolve-Path -Path (Join-Path -Path $docsDir -ChildPath '_Assets/Logo.svg')
} catch {
    Write-Error "Required files not found. Theme or favicon is missing in $_docs directory."
    exit 1
}

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
    $adocFiles = Get-ChildItem -Path $srcDir -Include *.adoc -Recurse -File 
    # Add the root README.adoc
    $adocFiles += Get-Item -Path (Join-Path -Path $scriptDir -ChildPath '../README.adoc')
}

if (-not $adocFiles -or $adocFiles.Count -eq 0) {
    Write-Error "No AsciiDoc files found. Exiting."
    exit 0
}

# Build attribute list
$attrs = @("source-highlighter=rouge")

$themePath = ($themePathObj.Path) -replace '\\', '/'
$attrs += "stylesheet=$themePath"

$favPath = ($favIconPathObj.Path) -replace '\\', '/'
$attrs += "favicon=$favPath"

# Build asciidoctor arguments
$attrArgs = $attrs | ForEach-Object { "-a $_" } | Join-String " "

foreach ($file in $adocFiles) {
    try {
        $filePath = $file.FullName
        Write-Output "Processing $filePath"

        # Call asciidoctor
        # Use --trace for verbose debugging if needed
        asciidoctor "$filePath" $attrArgs --trace
    } catch {
        Write-Error "Failed to process $($file.FullName): $_"
    }
}
