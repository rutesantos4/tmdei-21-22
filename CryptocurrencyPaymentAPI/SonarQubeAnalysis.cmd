dotnet test ../IntegrationTests/IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet test ../UnitTests/UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet sonarscanner begin /k:"fb1c385d78137a268807d5ed2aadcec09ba445b0" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="fb1c385d78137a268807d5ed2aadcec09ba445b0" /d:sonar.cs.opencover.reportsPaths="..\UnitTests\coverage.opencover.xml,..\IntegrationTests\coverage.opencover.xml" /d:sonar.coverage.exclusions="**Tests*.cs"
dotnet build
dotnet sonarscanner end /d:sonar.login="fb1c385d78137a268807d5ed2aadcec09ba445b0"
