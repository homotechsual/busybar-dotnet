<#
.SYNOPSIS
    Regenerates the API reference section of the Docusaurus site (website/docs/api) from the
    BusyBar library's XML doc comments.

.DESCRIPTION
    1. Builds src/BusyBar in Release (which produces BusyBar.xml alongside BusyBar.dll).
    2. Clears the previous website/docs/api output, so removed/renamed types don't leave stale pages.
    3. Runs the `xmldoc2md` local dotnet tool (see .config/dotnet-tools.json) against the built
       assembly, targeting the Docusaurus platform preset with a flat structure so front matter and
       file layout are ready to serve as-is. (Every type in this library lives in one namespace,
       Busy.Bar, so xmldoc2md's "tree" structure only adds a redundant "busy/bar/" folder level —
       and its generated doc `id`s then contain slashes, which Docusaurus rejects. Flat avoids both.)

    Re-run this any time the library's public API or XML doc comments change, before building or
    deploying the website.

.EXAMPLE
    pwsh ./scripts/generate-api-docs.ps1
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$libraryProject = Join-Path $repoRoot 'src/BusyBar/BusyBar.csproj'
$builtAssembly = Join-Path $repoRoot 'src/BusyBar/bin/Release/net10.0/BusyBar.dll'
$apiDocsOutput = Join-Path $repoRoot 'website/docs/api'

Write-Host "==> Building $libraryProject (Release)"
dotnet build $libraryProject -c Release
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed with exit code $LASTEXITCODE" }

if (-not (Test-Path $builtAssembly)) {
    throw "Expected built assembly not found at $builtAssembly"
}

if (Test-Path $apiDocsOutput) {
    Write-Host "==> Clearing previous output at $apiDocsOutput"
    Remove-Item -Recurse -Force $apiDocsOutput
}

Write-Host "==> Generating API docs from $builtAssembly"
Push-Location $repoRoot
try {
    dotnet tool run xmldoc2md $builtAssembly --output $apiDocsOutput --platform docusaurus --structure flat
    if ($LASTEXITCODE -ne 0) { throw "xmldoc2md failed with exit code $LASTEXITCODE" }
}
finally {
    Pop-Location
}

# xmldoc2md emits bare `<br>` on "Inheritance"/"Implements" lines. That's valid HTML but invalid
# MDX/JSX (void elements must self-close), and Docusaurus v3 compiles .md files as MDX — so an
# unpatched build fails with "Expected a closing tag for `<br>`". Patch it in place.
Write-Host "==> Patching <br> -> <br/> for MDX compatibility"
Get-ChildItem -Path $apiDocsOutput -Filter '*.md' -Recurse | ForEach-Object {
    (Get-Content -Path $_.FullName -Raw) -replace '<br>', '<br/>' | Set-Content -Path $_.FullName -NoNewline
}

# xmldoc2md renders enum members as a Markdown table ("| Name | Value | Description |"), not
# headings, so <see cref="Enum.Member"/> cross-references it generates (e.g. "#key") point at an
# anchor that doesn't exist — Docusaurus only auto-generates anchors for real headings. Rather than
# just suppressing the resulting warning, give those links something real to land on: inject an
# HTML anchor with a matching id into each field-table row. Data rows are distinguished from the
# table's header/separator rows by having a numeric second column (the enum's underlying value).
Write-Host "==> Adding real anchors to enum field tables so 'see cref' links resolve"
$fieldRowPattern = '(?m)^\| (\w+) \| (-?\d+) \| '
$fieldRowEvaluator = [System.Text.RegularExpressions.MatchEvaluator] {
    param($match)
    $name = $match.Groups[1].Value
    $value = $match.Groups[2].Value
    $anchorId = $name.ToLowerInvariant()
    "| <a id=""$anchorId""></a>$name | $value | "
}
Get-ChildItem -Path $apiDocsOutput -Filter '*.md' -Recurse | ForEach-Object {
    $content = Get-Content -Path $_.FullName -Raw
    [regex]::Replace($content, $fieldRowPattern, $fieldRowEvaluator) | Set-Content -Path $_.FullName -NoNewline
}

# xmldoc2md's "Inheritance A -> B -> C" line always ends with the current type linking to itself
# (e.g. on busy.bar.accountbackend.md: "-> [AccountBackend](./busy.bar.accountbackend.md)") — a
# link that just reloads the page you're already on. Strip that one self-referencing link per file
# down to plain text; every other link on the page is left untouched.
Write-Host "==> Removing self-referencing inheritance-chain links"
Get-ChildItem -Path $apiDocsOutput -Filter '*.md' -Recurse | ForEach-Object {
    $ownLinkPattern = "\[([^\]]+)\]\(\./$([regex]::Escape($_.BaseName))\.md\)"
    (Get-Content -Path $_.FullName -Raw) -replace $ownLinkPattern, '$1' | Set-Content -Path $_.FullName -NoNewline
}

Write-Host "==> Done. API docs written to $apiDocsOutput"
