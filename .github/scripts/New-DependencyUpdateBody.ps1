param(
    [Parameter(Mandatory)]
    [string] $OutputPath,
    [string] $BaseRef = 'HEAD',
    [string] $PackageCachePath = $(if ($env:NUGET_PACKAGES) { $env:NUGET_PACKAGES } else { Join-Path ([Environment]::GetFolderPath('UserProfile')) '.nuget/packages' })
)

$ErrorActionPreference = 'Stop'

function Get-PackageVersions([string] $Content, [string] $Source) {
    # Git stdout can retain a BOM, and file reads can leave one after a doubled BOM.
    # Normalize both sources before PowerShell attempts XML conversion.
    try {
        $Document = [xml] $Content.TrimStart([char] 0xFEFF)
    } catch {
        throw "Could not parse package XML from ${Source}: $($_.Exception.Message)"
    }
    $versions = @{}
    foreach ($node in $Document.SelectNodes("//*[local-name()='PackageReference' or local-name()='PackageVersion']")) {
        $id = $node.GetAttribute('Include')
        if (-not $id) { $id = $node.GetAttribute('Update') }
        $version = $node.GetAttribute('VersionOverride')
        if (-not $version) { $version = $node.GetAttribute('Version') }
        if (-not $version) {
            $child = $node.SelectSingleNode("*[local-name()='VersionOverride' or local-name()='Version']")
            if ($child) { $version = $child.InnerText }
        }
        if (-not $id -or -not $version) { continue }

        # Keep conditional references distinct when a project declares a package twice.
        $conditions = @($node.SelectNodes('ancestor-or-self::*[@Condition]') | ForEach-Object { $_.GetAttribute('Condition') })
        $key = @($node.LocalName, $id) + $conditions | ConvertTo-Json -Compress
        $versions[$key] = [pscustomobject]@{ Id = $id; Version = $version }
    }
    return $versions
}

function Format-Cell([string] $Value) {
    return [System.Net.WebUtility]::HtmlEncode($Value).Replace('|', '&#124;').Replace('`', '&#96;').Replace("`r", '').Replace("`n", ' ')
}

function Get-ReleaseNotes([string] $Package, [string] $Version) {
    # Only concrete package versions have a cache entry (MSBuild expressions do not).
    if ($Package -notmatch '^[A-Za-z0-9_.-]+$' -or $Version -notmatch '^\d+(\.\d+){1,3}(-[A-Za-z0-9.-]+)?(\+[A-Za-z0-9.-]+)?$') { return }
    $cacheVersion = ($Version -split '\+')[0].ToLowerInvariant()
    $id = $Package.ToLowerInvariant()
    $path = Join-Path $PackageCachePath "$id/$cacheVersion/$id.nuspec"
    try {
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { return }
        $document = [xml] (Get-Content -LiteralPath $path -Raw).TrimStart([char] 0xFEFF)
        $node = $document.SelectSingleNode("/*[local-name()='package']/*[local-name()='metadata']/*[local-name()='releaseNotes']")
        if ($node) { return $node.InnerText.Trim() }
    } catch {
        Write-Warning "Could not read release notes for $Package ${Version}: $($_.Exception.Message)"
    }
}

$files = @(git diff --name-only --diff-filter=M $BaseRef -- 'src/Avalonia.Samples/*.csproj' 'src/Avalonia.Samples/*.fsproj' 'src/Avalonia.Samples/*.props')
if ($LASTEXITCODE -ne 0) { throw 'Could not list changed dependency files.' }

$updates = @{}
foreach ($file in $files) {
    $original = git show "${BaseRef}:$file" | Out-String
    if ($LASTEXITCODE -ne 0) { throw "Could not read the original version of $file." }
    $before = Get-PackageVersions -Content $original -Source "${BaseRef}:$file"
    $after = Get-PackageVersions -Content (Get-Content -LiteralPath $file -Raw) -Source "working tree:$file"
    foreach ($key in $after.Keys) {
        if (-not $before.ContainsKey($key) -or $before[$key].Version -eq $after[$key].Version) { continue }
        $package = $after[$key].Id
        $oldVersion = $before[$key].Version
        $newVersion = $after[$key].Version
        $group = @($package, $oldVersion, $newVersion) | ConvertTo-Json -Compress
        if (-not $updates.ContainsKey($group)) {
            $updates[$group] = [pscustomobject]@{
                Package = $package
                From = $oldVersion
                To = $newVersion
                Files = [System.Collections.Generic.List[string]]::new()
            }
        }
        if (-not $updates[$group].Files.Contains($file)) { $updates[$group].Files.Add($file) }
    }
}

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('Updates NuGet dependencies within their current major versions. Major upgrades are handled separately through tracking issues.')
$lines.Add('')
$lines.Add('## Package changes')
$lines.Add('')
if ($updates.Count -gt 0) {
    $lines.Add('| Package | From | To | Projects / central package files |')
    $lines.Add('| --- | --- | --- | --- |')
    foreach ($update in ($updates.Values | Sort-Object Package, From, To)) {
        $paths = ($update.Files | Sort-Object | ForEach-Object { '<code>{0}</code>' -f (Format-Cell $_) }) -join '<br>'
        $packageUrl = 'https://www.nuget.org/packages/{0}/{1}' -f [uri]::EscapeDataString($update.Package), [uri]::EscapeDataString($update.To)
        $lines.Add(('| [{0}]({4}) | {1} | {2} | {3} |' -f (Format-Cell $update.Package), (Format-Cell $update.From), (Format-Cell $update.To), $paths, $packageUrl))
    }
} else {
    $lines.Add('No explicit package version changes were found in project or central package files. Review the diff for other changes before merging.')
}
if ($updates.Count -gt 0) {
    $lines.Add('')
    $lines.Add('## Release notes')
    $lines.Add('')
    $lines.Add('Notes from the restored target versions; skipped releases may contain additional changes.')
    $lines.Add('')
    # Leave room in the PR body for the package table and review checklist.
    $notesBudget = 16000
    foreach ($release in ($updates.Values | Sort-Object Package, To -Unique)) {
        $packageUrl = 'https://www.nuget.org/packages/{0}/{1}' -f [uri]::EscapeDataString($release.Package), [uri]::EscapeDataString($release.To)
        $lines.Add(('- <code>{0} {1}</code> — [NuGet package]({2})' -f (Format-Cell $release.Package), (Format-Cell $release.To), $packageUrl))
        $notes = Get-ReleaseNotes -Package $release.Package -Version $release.To
        if (-not $notes) {
            $lines.Add('  Release notes unavailable in restored package metadata; see the package page.')
        } elseif ($notesBudget -le 0) {
            $lines.Add('  Additional notes omitted to keep the PR body short; see the package page.')
        } else {
            $uri = $null
            if ($notes -notmatch '\s' -and [uri]::TryCreate($notes, [UriKind]::Absolute, [ref] $uri) -and $uri.Scheme -in @('http', 'https')) {
                $rendered = '  <a href="{0}">Release notes</a>' -f [System.Net.WebUtility]::HtmlEncode($uri.AbsoluteUri)
            } else {
                $limit = [Math]::Min(1500, $notesBudget)
                if ($notes.Length -gt $limit) { $notes = $notes.Substring(0, $limit) + ' … (truncated; see package page)' }
                $rendered = '  <pre>{0}</pre>' -f [System.Net.WebUtility]::HtmlEncode($notes)
            }
            $lines.Add($rendered)
            $notesBudget -= $rendered.Length
        }
        $lines.Add('')
    }
}
$lines.Add('')
$lines.Add('## Validation')
$lines.Add('')
$lines.Add('- Package restore completed for all sample projects after the updates.')
$lines.Add('- Builds and tests are not run by this workflow.')
$lines.Add('')
$lines.Add('## Review')
$lines.Add('')
$lines.Add('- [ ] Check release notes for the updated packages.')
$lines.Add('- [ ] Verify builds and tests, including affected Desktop, Android, iOS, and Browser targets.')
$lines.Add('- [ ] Smoke-test the affected samples for behavior changes.')
if ($env:GITHUB_SERVER_URL -and $env:GITHUB_REPOSITORY -and $env:GITHUB_RUN_ID) {
    $lines.Add('')
    $lines.Add("Generated by the [Update dependencies workflow]($env:GITHUB_SERVER_URL/$env:GITHUB_REPOSITORY/actions/runs/$env:GITHUB_RUN_ID).")
}
Set-Content -LiteralPath $OutputPath -Value $lines -Encoding utf8
