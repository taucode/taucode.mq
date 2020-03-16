dotnet restore

dotnet build --configuration Debug
dotnet build --configuration Release

dotnet test -c Debug .\tests\TauCode.Mq.Tests\TauCode.Mq.Tests.csproj
dotnet test -c Release .\tests\TauCode.Mq.Tests\TauCode.Mq.Tests.csproj

nuget pack nuget\TauCode.Mq.nuspec
