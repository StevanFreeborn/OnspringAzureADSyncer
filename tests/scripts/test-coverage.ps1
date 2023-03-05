$RootPath = Split-Path $PSScriptRoot -Parent

Remove-Item -Path $RootPath\TestResults\* -Recurse -Force

dotnet test --collect:"XPlat Code Coverage;Include=[OnspringAzureADSyncer]*"

reportgenerator `
-reports:$RootPath\TestResults\*\coverage.cobertura.xml `
-targetdir:$RootPath\TestResults\coveragereport `
-reporttypes:Html_Dark;

$testPath = (Split-Path (Split-Path $PSScriptRoot -parent) -leaf)

live-server --open=$testPath\TestResults\coveragereport\