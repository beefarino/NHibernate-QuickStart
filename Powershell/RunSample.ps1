cls
Set-ExecutionPolicy bypass -force

[reflection.assembly]::loadFrom("NHibernate.DriveProvider.dll") | import-module

[reflection.assembly]::loadFrom("people.domain.dll") | import-module

[reflection.assembly]::loadFrom("people.dataaccess.dll") | import-module

new-psdrive -psprovider NHibernate -name People -root "People.Domain.Person" -dataAccessAssembly "GeekNight.Core.dll" -domainAssembly "GeekNight.Domain.dll"

Get-Item People:\"People.Domain.Person,People.Domain" -filter "ID > 0"
    
$geek = New-Object -typename GeekNight.Domain.Geek -prop @{ EmailAddress="larry@lambda.org"; Name="Larry" }
    
New-Item -type GeekNight.Domain.Geek -path GeekNightSQLite:\"GeekNight.Domain.Geek,GeekNight.Domain" -value $geek

Get-Item GeekNightSQLite:\"GeekNight.Domain.Geek,GeekNight.Domain" -filter "EmailAddress = `"larry@lambda.org`""

Copy-Item -Filter "EmailAddress = `"larry@lambda.org`"" GeekNightSQLite:\"GeekNight.Domain.Geek,GeekNight.Domain" -destination GeekNightMSSQL:\"GeekNight.Domain.Geek,GeekNight.Domain"
    
read-host "All finished! Press [Enter] to exit."