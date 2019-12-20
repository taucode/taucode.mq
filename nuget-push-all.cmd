rem pushes all (actually, one, because there should be only one) packages to the feed
forfiles /p nuget-pack /m *.nupkg /c "cmd /c call ..\nuget-push-z1 @file"