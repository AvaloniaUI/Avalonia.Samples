# Compiles each .adoc file in all subdirectories (recursively) into a PDF using Asciidoctor-pdf.
# If specific files are provided as arguments, only those files will be processed.

param(
    [Parameter(Mandatory=$false, ValueFromRemainingArguments=$true)]
    [string[]]$InputFiles
)

# Find all .adoc files recursively
if ($InputFiles -and $InputFiles.Count -gt 0) {
    $adocFiles = $InputFiles | ForEach-Object { Get-Item $_ }
} else {
    $adocFiles = Get-ChildItem -Path ../. -Filter *.adoc -Recurse
}

$themePath = Join-Path -Path (Resolve-Path ../_docs) -ChildPath "avalonia-docs-theme.yml"

foreach ($file in $adocFiles) {
    $pdfPath = [System.IO.Path]::ChangeExtension($file.FullName, ".pdf")
    Write-Host "Compiling $($file.FullName) to $pdfPath"

    asciidoctor-pdf "$($file.FullName)" -r asciidoctor-diagram -a pdf-theme="$themePath" -a allow-uri-read=true -a source-highlighter=rouge -a compress=screen -o "$pdfPath" --trace
}