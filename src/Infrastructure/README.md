# Infrastructure as Code

## Pulumi

Pulumi is a SDK to describe infrastructure as code in .NET an deploy it with the Pulumi CLI tool to Azure.

## Configuration

The configuration for each environment can be found in the Pulumi.{environment}.yaml files. All the secret configurations must be manually set:

```powershell
pulumi config set --secret <key> <value>
```

Pulumi makes sure to save secrets and credentials encrypted in the state file.

## Azure Login

Pulumi uses the Azure CLI login and the current default account. Before configuring and deploying make sure that you are logged in and connected to the correct Azure subscription:

```powershell
az login
az account list
az account set -s <subscriptionId>
```

### Service Principal

In order to create the resources in Azure within an Azure pipeline create a service connection in Azure DevOps via a service principal. The service principal needs additional permissions to manage the Azure Active Directory. Select the service principal in the AAD applications and grant the application permissions in Azure Active Directory (legacy): Directory.ReadWrite.All, Application.ReadWrite.All.

## State

The current state of the environment (stack) is saved to a state file. This state file is stored locally by default or can be stored in an Azure Blob Storage.

### Setup Azure Blob Storage

```powershell
$env:AZURE_STORAGE_ACCOUNT = 'webapiinfrastructurestg'
$env:AZURE_STORAGE_KEY = (az storage account keys list -n $env:AZURE_STORAGE_ACCOUNT --query "[0].value" -o tsv)
pulumi login azblob://state
```

The configuration and state is now saved to the Azure Blob Storage container.

## Deploy

Set the keephrase for the environment:

```powershell
$environment = 'dev' # dev, prev, prod
$env:PULUMI_CONFIG_PASSPHRASE = (az keyvault secret show -n PulumiPassphrase --vault-name webapi-$environment-euw-kv --query value -o tsv)
```

The given environment can be deployed as follow:

```powershell
pulumi up -s <dev/prev/prod>
```

## Destroy

The given environment can be destroyed as follow:

```powershell
pulumi destroy -s <dev/prev/prod>
```

**Make sure that you know what you are doing before destroying an environment.**
