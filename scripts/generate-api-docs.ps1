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

Write-Host "==> Done. API docs written to $apiDocsOutput"
