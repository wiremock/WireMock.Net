rem https://github.com/StefH/GitHubReleaseNotes

@echo off
setlocal

for /f "usebackq delims=" %%v in (`powershell -NoProfile -Command "[xml]$props = Get-Content 'Directory.Build.props'; $props.Project.PropertyGroup | ForEach-Object { $_.VersionPrefix } | Where-Object { $_ } | Select-Object -First 1"`) do set version=%%v

if "%version%"=="" (
	echo Unable to determine VersionPrefix from Directory.Build.props.
	exit /b 1
)

GitHubReleaseNotes --output CHANGELOG.md --skip-empty-releases --exclude-labels wontfix test question invalid doc duplicate example environment --version %version% --token %GH_TOKEN%
if errorlevel 1 exit /b %errorlevel%

GitHubReleaseNotes --output PackageReleaseNotes.txt --skip-empty-releases --exclude-labels wontfix test question invalid doc duplicate example environment --template PackageReleaseNotes.template --version %version% --token %GH_TOKEN%
if errorlevel 1 exit /b %errorlevel%