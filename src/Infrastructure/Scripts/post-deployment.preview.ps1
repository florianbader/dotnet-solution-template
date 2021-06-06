param (
    [string] [Parameter(Mandatory = $true)] $environment,
    [string] [Parameter(Mandatory = $true)] $location
)

$rootDirectory = "$PSScriptRoot/../../../"

$pulumiOutputs = (Get-Content "$rootDirectory/src/Infrastructure/outputs.json" | ConvertFrom-Json)
