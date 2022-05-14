# Cryptocurrency in Online Payments: Moving towards digital currency for digital payments

The present repository contains all source code related to the thesis submitted in partial fulfilment of the requirements for the degree of Master of Informatics, Specialisation Area of Software Engineering at ISEP (Instituto Superior de Engenharia do Porto).

The context of this thesis lies in the exploration activity of an emerging technological area. Its main objective is the development of a prototype capable of supporting the cryptocurrency Bitcoin. Thus, the following activities are planned:

- Identify approaches to cryptocurrency transaction processing to gain aunderstanding of the cryptocurrency payments business.
- Compare the approaches according to some criteria, namely theisecurity, ease of supporting more cryptocurrencies besides bitcoin, animplementation costs, both monetary and time.
- Analyse the suitability of different technologies and tools for a choseapproach.
- Develop a prototype that facilitates the integration with otheapplications,
- Validate the solution in terms of some quality attributes.  



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
