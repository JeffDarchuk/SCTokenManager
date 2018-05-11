$Webroot = "C:\inetpub\wwwroot\sc901.local"

Copy-Item "$PSScriptRoot\Source\TokenManager\Tokens.config" "$(New-Item "$Webroot\App_Config\Include\TokenManagerTest" -ItemType directory -Force)/Tokens.config" -Force
Get-ChildItem "$PSScriptRoot\Source\TokenManager\bin\Debug" | Foreach-Object{
	Write-Host "moving $($_.FullName) to bin root" -ForegroundColor Green
	Copy-Item $_.FullName "$Webroot\bin\$(Split-Path $_.FullName -Leaf)" -Force
}

Copy-Item "$PSScriptRoot\TokenManagerDemo\TokenDemo.config" "$(New-Item "$Webroot\App_Config\Include\TokenManagerTest" -ItemType directory -Force)/TokenDemo.config" -Force
Get-ChildItem "$PSScriptRoot\TokenManagerDemo\bin\Debug" | Foreach-Object{
	Write-Host "moving $($_.FullName) to bin root" -ForegroundColor Green
	Copy-Item $_.FullName "$Webroot\bin\$(Split-Path $_.FullName -Leaf)" -Force
}