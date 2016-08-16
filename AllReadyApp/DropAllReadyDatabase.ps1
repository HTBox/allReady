Import-Module SQLPS -DisableNameChecking

Write-Host "Dropping AllReady Database"

$AllReady = "AllReady"
$SQLInstanceName = "(LocalDB)\MSSQLLocalDB"
$Server = New-Object -TypeName Microsoft.SqlServer.Management.Smo.Server -ArgumentList $SQLInstanceName

if($Server.Databases[$AllReady])
{
	Write-Host "   " $AllReady
	$Server.KillDatabase($AllReady)
}
