dotnet restore

dotnet build --configuration Debug
dotnet build --configuration Release

dotnet test -c Debug .\test\TauCode.Mq.Tests\TauCode.Mq.Tests.csproj
dotnet test -c Release .\test\TauCode.Mq.Tests\TauCode.Mq.Tests.csproj

nuget pack nuget\TauCode.Mq.nuspec
