dotnet restore

dotnet build TauCode.Mq.sln -c Debug
dotnet build TauCode.Mq.sln -c Release

dotnet test TauCode.Mq.sln -c Debug
dotnet test TauCode.Mq.sln -c Release

nuget pack nuget\TauCode.Mq.nuspec