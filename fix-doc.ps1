# fix-wiremock-urls.ps1
Get-ChildItem -Path . -Filter *.md -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    # Replace all URLs starting with  $matches[0].ToLower()  with their lowercase version
    $fixed = $content -replace '(https://wiremock\.org/dotnet/[^)\s]*)', { $matches[0].ToLower() }
    if ($content -ne $fixed) {
        Set-Content $_.FullName $fixed
        Write-Host "Updated:" $_.FullName
    }
}
