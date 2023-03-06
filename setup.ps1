function Execute ($action) {
    if($action -eq "urlacl") {
        UrlAcl
        return
    }

    if($action -eq "localdb") {
        CreateLocalDb
        return
    }

    Write-Host "Unknown action: $action. Known actions are 'urlacl' and 'localdb'"
}

function Get-DatabaseScript($dataDirectory, $databaseName) {
    $dbLog = $databaseName + "_log.ldf"
    return "IF EXISTS (SELECT * FROM sys.databases WHERE name = '$databaseName')
    BEGIN
      ALTER DATABASE [$databaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
      DROP DATABASE [$databaseName]
    END
    CREATE DATABASE [$databaseName]
    ON (
      NAME = [$databaseName],
      FILENAME = '$dataDirectory\$databaseName.mdf'
    )
    LOG ON (
      NAME = [$dbLog],
      FILENAME = '$dataDirectory\$dbLog'
    )"
}

function UrlAcl {
    Invoke-Expression "netsh http add urlacl url=http://+:42480/ user=Everyone"
}

function CreateLocalDb {
    $currentDirectory = Get-Location
    $dataDirectory = Join-Path -Path $currentDirectory -ChildPath "src\Host\Data"
    New-Item -Path $dataDirectory -ItemType Directory -Force
    $databaseScript = Get-DatabaseScript $dataDirectory "PromocodeService"

    Invoke-Expression "sqllocaldb create ECommerce"
    Invoke-Expression "sqlcmd -S ""(LocalDB)\ECommerce"" -Q ""$databaseScript"""
}

Execute $args