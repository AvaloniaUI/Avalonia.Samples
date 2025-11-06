# Compiles each .adoc file in all subdirectories (recursively) into a HTML-file using Asciidoctor-CLI.
# If specific files are provided as arguments, only those files will be processed.

param(
    [Parameter(Mandatory=$false, ValueFromRemainingArguments=$true)]
    [string[]]$InputFiles
)

# Resolve script directory reliably
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$docsDir = Join-Path -Path $scriptDir -ChildPath '../_docs'
$srcDir = Join-Path -Path $scriptDir -ChildPath '../src'

# Try to resolve theme and favicon, but continue if missing
$themePathObj = Resolve-Path -Path (Join-Path -Path $docsDir -ChildPath 'avalonia-docs-theme.css') -ErrorAction SilentlyContinue
$favIconPathObj = Resolve-Path -Path (Join-Path -Path $docsDir -ChildPath '_Assets/Logo.svg') -ErrorAction SilentlyContinue

if ($InputFiles -and $InputFiles.Count -gt 0) {
    $adocFiles = $InputFiles | ForEach-Object {
        try { Get-Item $_ -ErrorAction SilentlyContinue } catch { $null }
    } | Where-Object { $_ -ne $null }
} else {
    # Find .adoc files recursively in src
    $adocFiles = Get-ChildItem -Path $srcDir -Include *.adoc -Recurse -File -ErrorAction SilentlyContinue

    # Try common README casings at repo root
    $repoRoot = Join-Path -Path $scriptDir -ChildPath '..'
    $readme = Get-ChildItem -Path $repoRoot/README.adoc
    if ($readme) { $adocFiles += $readme }
}

if (-not $adocFiles -or $adocFiles.Count -eq 0) {
    Write-Output "No AsciiDoc files found. Exiting."
    exit 0
}

foreach ($file in $adocFiles) {
    try {
        $filePath = $file.FullName

        # Build attribute list
        $attrs = @("source-highlighter=rouge")
        if ($themePathObj) {
            $themePath = ($themePathObj.Path) -replace '\\', '/'
            $attrs += "stylesheet=$themePath"
        } else {
            Write-Output "Warning: theme file not found at expected location ($docsDir/avalonia-docs-theme.css). Continuing without custom stylesheet."
        }
        if ($favIconPathObj) {
            $favPath = ($favIconPathObj.Path) -replace '\\', '/'
            $attrs += "favicon=$favPath"
        }

        # Build asciidoctor arguments
        $attrArgs = $attrs | ForEach-Object { "-a $_" } | Join-String " "
        Write-Output "Processing $filePath"
        Write-Output "Using attributes: $($attrs -join ', ')"

        # Call asciidoctor
        # Use --trace for verbose debugging if needed
        asciidoctor "$filePath" $attrArgs --verbose
    } catch {
        Write-Error "Failed to process $($file.FullName): $_"
    }
}