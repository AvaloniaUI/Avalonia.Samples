# Compiles each .adoc file in all subdirectories (recursively) into a HTML-file using Asciidoctor-CLI.
# If specific files are provided as arguments, only those files will be processed.

param(
    [Parameter(Mandatory=$false, ValueFromRemainingArguments=$true)]
    [string[]]$InputFiles
)

$themePath = Join-Path -Path (Resolve-Path ../_docs) -ChildPath "avalonia-docs-theme.css"

# Find all .adoc files recursively
if ($InputFiles -and $InputFiles.Count -gt 0) {
    $adocFiles = $InputFiles | ForEach-Object { Get-Item $_ }
} else {
    $adocFiles = Get-ChildItem -Path ../src/. -Filter *.adoc -Recurse
}

foreach ($file in $adocFiles) {
    asciidoctor "$($file.FullName)" -a source-highlighter=rouge -a stylesheet=$themePath --verbose
}