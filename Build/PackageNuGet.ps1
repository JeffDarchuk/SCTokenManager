param($scriptRoot)

$ErrorActionPreference = "Stop"

$programFilesx86 = ${Env:ProgramFiles(x86)}
$msBuild = "$programFilesx86\MSBuild\14.0\bin\msbuild.exe"
$nuGet = "$scriptRoot..\tools\NuGet.exe"
$solution = "$scriptRoot\..\TokenManager.sln"

& $nuGet restore $solution
& $msBuild $solution /p:Configuration=Release /t:Rebuild /m

$tmAssembly = Get-Item "$scriptRoot\..\Source\TokenManager\bin\Release\TokenManager.dll" | Select-Object -ExpandProperty VersionInfo
$targetAssemblyVersion = $tmAssembly.ProductVersion

& $nuGet pack "$scriptRoot\TokenManager.nuget\TokenManager.nuspec" -version $targetAssemblyVersion

& $nuGet pack "$scriptRoot\..\Source\TokenManager\TokenManager.csproj" -Symbols -Prop "Configuration=Release"