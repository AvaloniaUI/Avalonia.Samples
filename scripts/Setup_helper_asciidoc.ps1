# Ensure Ruby is installed
if (-not (Get-Command "ruby" -ErrorAction SilentlyContinue)) {
    Write-Error "Ruby is not installed. Please install Ruby first."
    exit 1


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

# Ensure ImageMagick is installed for prawn-gmagick to work
if (-not (Get-Command "magick" -ErrorAction SilentlyContinue)) {
    Write-Host "ImageMagick is not installed. Please install ImageMagick (https://imagemagick.org/script/download.php) and ensure 'magick' is in your PATH."
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