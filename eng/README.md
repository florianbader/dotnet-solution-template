# Azure Pipelines

## Prerequisites

In order to setup the pipeline you need to install the Azure DevOps CLI.

```cmd
az extension add --name azure-devops
```

## Login

Login to you team account and team project:

```cmd
az devops configure --defaults organization=https://dev.azure.com/contoso project=ContosoWebApp
```

## Create pipeline

```cmd
$repoName = 'WebApi'
az pipelines folder create --path WebApi
az pipelines create --name 'WebApi.CI' --folder-path WebApi --repository $repoName --repository-type tfsgit --branch master --skip-run --yaml-path /eng/webapi/pipeline.yml
az pipelines create --name 'WebApi.PR' --folder-path WebApi --repository $repoName --repository-type tfsgit --branch master --skip-run --yaml-path /eng/webapi/build-pipeline.yml

$pipelineId = (az pipelines list --name WebApi.PR --query "[0].id" -o tsv)
$repoId = (az repos list --query "[?name=='$repoName'].id" -o tsv)
az repos policy build create --blocking true --branch master --build-definition-id $pipelineId --display-name 'Build' --enabled true --manual-queue-only false --queue-on-source-update-only false --repository-id $repoId --valid-duration 0 --path-filter /src/WebApi/**/*
```
