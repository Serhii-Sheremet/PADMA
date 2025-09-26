# Explicit path to your project root
$ProjectRoot  = "E:\Projects\AstroPlus\PADMA"
$SnapshotPath = Join-Path $ProjectRoot "PROJECT_SNAPSHOT.md"

Write-Host "Project root: $ProjectRoot"
Write-Host "Snapshot file will be saved as: $SnapshotPath"

# Ensure snapshot file starts clean
Remove-Item $SnapshotPath -ErrorAction SilentlyContinue

# Create header (UTF-8 without BOM)
[System.IO.File]::WriteAllText(
    $SnapshotPath,
    "Project Snapshot`r`n`r`nGenerated on $(Get-Date)`r`n`r`n",
    (New-Object System.Text.UTF8Encoding $false)
)

# Collect all relevant files, excluding bin/ and obj/
$files = Get-ChildItem -Path $ProjectRoot -Recurse -Include *.cs, *.xaml, *.csproj, *.sln -File -ErrorAction SilentlyContinue |
         Where-Object { $_.FullName -notmatch "\\(bin|obj)\\" }

Write-Host "Files found: $($files.Count)"

# Append each file content to snapshot
foreach ($file in $files | Sort-Object FullName) {
    $relativePath = $file.FullName.Replace($ProjectRoot, ".")
    $content = Get-Content $file.FullName -Raw
    $extension = [System.IO.Path]::GetExtension($file.FullName).TrimStart(".")

    $block = "## $relativePath`r`n```$extension`r`n$content`r`n``` `r`n`r`n"

    [System.IO.File]::AppendAllText(
        $SnapshotPath,
        $block,
        (New-Object System.Text.UTF8Encoding $false)
    )
}

Write-Host "Snapshot created: $SnapshotPath"
