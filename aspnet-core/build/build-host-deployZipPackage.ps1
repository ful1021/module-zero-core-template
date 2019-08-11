# COMMON PATHS

$buildFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $buildFolder "../"
$outputFolder = Join-Path $buildFolder "outputs/HostZip"
$slnName = "AbpCompanyName.AbpProjectName.Web.Host.sln"
$zipPackageFullName = Join-Path $outputFolder "AbpCompanyName.AbpProjectName.Web.Host.zip"

## CLEAR ######################################################################

Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
New-Item -Path $outputFolder -ItemType Directory

## PUBLISH WEB HOST PROJECT ###################################################

Set-Location $slnFolder
dotnet build $slnName /p:WebPublishMethod=Package /nologo /p:PublishProfile=Release /p:DeployOnBuild=true  /p:PackageAsSingleFile=true /maxcpucount:1 /p:platform="Any CPU" /p:configuration="Release" /p:DesktopBuildPackageLocation=$zipPackageFullName

## FINALIZE ###################################################################

Set-Location $outputFolder