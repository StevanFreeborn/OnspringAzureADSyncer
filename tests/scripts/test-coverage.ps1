Remove-Item -Path TestResults -Recurse -Force

dotnet test --collect:"XPlat Code Coverage;Include=[OnspringAzureADSyncer]*"

dotnet reportgenerator `
-reports:TestResults\*\coverage.cobertura.xml `
-targetdir:TestResults\coveragereport `
-reporttypes:Html_Dark;

cd TestResults\coveragereport
live-server