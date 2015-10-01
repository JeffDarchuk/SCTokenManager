param($scriptRoot)

$msBuild = "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
$nuGet = "$scriptRoot..\Dependencies\NuGet.exe"
$solution = "$scriptRoot\..\TokenManager.sln"

& $nuGet restore $solution
& $msBuild $solution /p:Configuration=Release /t:Rebuild /m

$assembly = Get-Item "$scriptRoot\..\Source\TokenManager\bin\Release\TokenManager.dll" | Select-Object -ExpandProperty VersionInfo
$targetAssemblyVersion = $assembly.ProductVersion

& $nuGet pack "$scriptRoot\TokenManager.nuspec" -version $targetAssemblyVersion
