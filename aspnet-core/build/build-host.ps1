# COMMON PATHS

$buildFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $buildFolder "../"
$outputFolder = Join-Path $buildFolder "outputs"
$outputHostFolder = Join-Path $outputFolder "Host"
$webHostFolder = Join-Path $slnFolder "src/AbpCompanyName.AbpProjectName.Web.Host"

## CLEAR ######################################################################

Remove-Item $outputHostFolder -Force -Recurse -ErrorAction Ignore
New-Item -Path $outputHostFolder -ItemType Directory

## PUBLISH WEB HOST PROJECT ###################################################
Set-Location $webHostFolder
dotnet publish --output $outputHostFolder --configuration Release

## FINALIZE ###################################################################

Set-Location $outputFolder