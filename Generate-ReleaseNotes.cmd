rem https://github.com/StefH/GitHubReleaseNotes

SET version=1.18.0

GitHubReleaseNotes --output CHANGELOG.md --skip-empty-releases --exclude-labels wontfix test question invalid doc duplicate example environment --version %version% --token %GH_TOKEN%

GitHubReleaseNotes --output PackageReleaseNotes.txt --skip-empty-releases --exclude-labels test question invalid doc duplicate example environment --template PackageReleaseNotes.template --version %version% --token %GH_TOKEN%