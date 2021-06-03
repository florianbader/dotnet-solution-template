param (
    [string] [Parameter(Mandatory = $true)] $environment
)

$sql = @"
DROP USER IF EXISTS [webapi-$environment-euw-app];
CREATE USER [webapi-$environment-euw-app] WITH default_schema = [dbo] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [webapi-$environment-euw-app];
ALTER ROLE db_datawriter ADD MEMBER [webapi-$environment-euw-app];
"@

sqlcmd -I -G -S "webapi-$environment-euw-sqls.database.windows.net" -d "webapi-$environment-euw-sqld" -Q $sql
