param($scriptRoot)

$ErrorActionPreference = "Stop"

function Resolve-MsBuild {
	$msb2017 = Resolve-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\*\*\MSBuild\*\bin\msbuild.exe" -ErrorAction SilentlyContinue
	if($msb2017) {
		Write-Host "Found MSBuild 2017 (or later)."
		Write-Host $msb2017
		return $msb2017
	}

	$msBuild2015 = "${env:ProgramFiles(x86)}\MSBuild\14.0\bin\msbuild.exe"

	if(-not (Test-Path $msBuild2015)) {
		throw 'Could not find MSBuild 2015 or later.'
	}

	Write-Host "Found MSBuild 2015."
	
	Write-Host $msBuild2015

	return $msBuild2015
}

$msBuild = Resolve-MsBuild
$nuGet = "$scriptRoot..\tools\NuGet.exe"
$solution = "$scriptRoot\..\TokenManager.sln"

& $nuGet restore $solution
& $msBuild $solution /p:Configuration=Release /t:Rebuild /m

$tmAssembly = Get-Item "$scriptRoot\..\Source\TokenManager\bin\Release\TokenManager.dll" | Select-Object -ExpandProperty VersionInfo
$targetAssemblyVersion = $tmAssembly.ProductVersion

& $nuGet pack "$scriptRoot\TokenManager.nuget\TokenManager.nuspec" -version $targetAssemblyVersion

& $nuGet pack "$scriptRoot\..\Source\TokenManager\TokenManager.csproj" -Symbols -Prop "Configuration=Release"