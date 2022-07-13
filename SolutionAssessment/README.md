## SonarQube Analysis

### Pre requisites

1. [Sonarqube docker](https://docs.sonarqube.org/latest/setup/get-started-2-minutes) running (or locally running) with port `9000`;
2. [SonarScanner for .NET](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner-for-msbuild/) installed;
3. [Coverlet](https://github.com/coverlet-coverage/coverlet) package installed in the test project (`UnitTests` and `IntegrationTests`);
4. Sonarqube project created with the name `tmdei-21-22` (it should generate the token `fb1c385d78137a268807d5ed2aadcec09ba445b0`) and project key `fb1c385d78137a268807d5ed2aadcec09ba445b0`;


### Run SonarScanner

Run the following command in the project directory (`/CryptocurrencyPaymentAPI`)

1. Execute the tests and perform the code coverage of unit tests

		dotnet test ../UnitTests/UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

2. Execute the tests and perform the code coverage of unit tests

		dotnet test ../IntegrationTests/IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

3. Begin the SonarQube Analysis

		dotnet sonarscanner begin /k:"fb1c385d78137a268807d5ed2aadcec09ba445b0" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="fb1c385d78137a268807d5ed2aadcec09ba445b0" /d:sonar.cs.opencover.reportsPaths="..\UnitTests\coverage.opencover.xml,..\IntegrationTests\coverage.opencover.xml" /d:sonar.coverage.exclusions="**Tests*.cs"

4. Build the solution

		dotnet build

5. End the SonarQube Analysis and publish it

		dotnet sonarscanner end /d:sonar.login="fb1c385d78137a268807d5ed2aadcec09ba445b0"


## Snyk analysis

### Pre requisites

1. [Snyk](https://app.snyk.io/login?cta=login&loc=nav&page=homepage) account created;
2. Project configured on Snyk (this run the analysis based on the code available on the repository);
3. [Snyk plugin for Visual Studio](https://github.com/snyk/snyk-visual-studio-plugin#snyk-for-visual-studio) or [Snyk CLI](https://github.com/snyk/cli) installed (this is to run the analysis locally);
4. [Snyk JSON to HTML Mapper](https://github.com/snyk/snyk-to-html) installed;


### Run Analysis

It can be done using the
1. Visual Studio extension
2. Synk website
3. Snyk CLI executing (as administrator) in the project directory (`/CryptocurrencyPaymentAPI`)
		snyk auth  # this will require you to authenticate with the snyk online service
		snyk test


### Generate report

1. If you have the Visual Studio plugin find where the executable file is located (something like `%USER%\.vscode\extensions\snyk-security.snyk-vulnerability-scanner-1.2.18\snyk-win.exe`)
		%USER%\.vscode\extensions\snyk-security.snyk-vulnerability-scanner-1.2.18\snyk-win.exe test . --json | snyk-to-html -o report-snyk.html

2. If you have the Snyk CLI execute (as administrator)
			snyk test --json | snyk-to-html -o report-snyk.html
