# Compiles each .adoc file in all subdirectories (recursively) into a PDF using Asciidoctor-pdf.
# If specific files are provided as arguments, only those files will be processed.

param(
    [Parameter(Mandatory=$false, ValueFromRemainingArguments=$true)]
    [string[]]$InputFiles
)

# Ensure Ruby is installed
if (-not (Get-Command "ruby" -ErrorAction SilentlyContinue)) {
    Write-Error "Ruby is not installed. Please install Ruby first."
    exit 1
}

# Ensure asciidoctor-pdf is installed
if (-not (Get-Command "asciidoctor-pdf" -ErrorAction SilentlyContinue)) {
    Write-Host "Installing asciidoctor-pdf gem..."
    & gem install asciidoctor-pdf | Out-Host
    # Wait for the installation to complete before proceeding
    if (-not (Get-Command "asciidoctor-pdf" -ErrorAction SilentlyContinue)) {
        Write-Error "asciidoctor-pdf installation failed."
        exit 1
    }
}

# Ensure asciidoctor-diagram is installed
if (-not (gem list asciidoctor-diagram -i | Select-String "true")) {
    Write-Host "Installing asciidoctor-diagram gem..."
    & gem install asciidoctor-diagram | Out-Host
    # Wait for the installation to complete before proceeding
    if (-not (gem list asciidoctor-diagram -i | Select-String "true")) {
        Write-Error "asciidoctor-diagram installation failed."
        exit 1
    }
}

# Ensure prawn-gmagick is installed for image format support
if (-not (gem list prawn-gmagick -i | Select-String "true")) {
    Write-Host "Installing prawn-gmagick gem for image format support..."
    & gem install prawn-gmagick | Out-Host
}

# Ensure rouge is installed for syntax highlighting
if (-not (gem list rouge -i | Select-String "true")) {
    Write-Host "Installing rouge gem for syntax highlighting..."
    & gem install rouge | Out-Host
}

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

    asciidoctor-pdf "$($file.FullName)" -r asciidoctor-diagram -a pdf-theme="$themePath" -a allow-uri-read=true -a source-highlighter=rouge -o "$pdfPath" --trace
}