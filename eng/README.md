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

## Create web api pipeline

```cmd
$repoName = 'WebApi'
az pipelines folder create --path WebApi
az pipelines create --name 'WebApi.CI' --folder-path WebApi --repository $repoName --repository-type tfsgit --branch master --skip-run --yaml-path /eng/webapi/pipeline.yml
az pipelines create --name 'WebApi.PR' --folder-path WebApi --repository $repoName --repository-type tfsgit --branch master --skip-run --yaml-path /eng/webapi/build-pipeline.yml

$pipelineId = (az pipelines list --name WebApi.PR --query "[0].id" -o tsv)
$repoId = (az repos list --query "[?name=='$repoName'].id" -o tsv)
az repos policy build create --blocking true --branch master --build-definition-id $pipelineId --display-name 'Build' --enabled true --manual-queue-only false --queue-on-source-update-only false --repository-id $repoId --valid-duration 0 --path-filter /src/WebApi/**/*
```

## Create website pipeline

```cmd
$repoName = 'WebApi'
az pipelines folder create --path Website
az pipelines create --name 'Website.CI' --folder-path Website --repository $repoName --repository-type tfsgit --branch master --skip-run --yaml-path /eng/website/pipeline.yml
az pipelines create --name 'Website.PR' --folder-path Website --repository $repoName --repository-type tfsgit --branch master --skip-run --yaml-path /eng/website/build-pipeline.yml

$pipelineId = (az pipelines list --name Website.PR --query "[0].id" -o tsv)
$repoId = (az repos list --query "[?name=='$repoName'].id" -o tsv)
az repos policy build create --blocking true --branch master --build-definition-id $pipelineId --display-name 'Build' --enabled true --manual-queue-only false --queue-on-source-update-only false --repository-id $repoId --valid-duration 0 --path-filter /src/Website/**/*
```

## Create infrastructure pipeline

```cmd
$repoName = 'WebApi'
az pipelines folder create --path Infrastructure
az pipelines create --name 'Infrastructure.CI' --folder-path Infrastructure --repository $repoName --repository-type tfsgit --branch master --skip-run --yaml-path /eng/infrastructure/pipeline.yml
az pipelines create --name 'Infrastructure.PR' --folder-path Infrastructure --repository $repoName --repository-type tfsgit --branch master --skip-run --yaml-path /eng/infrastructure/build-pipeline.yml

$pipelineId = (az pipelines list --name Infrastructure.PR --query "[0].id" -o tsv)
$repoId = (az repos list --query "[?name=='$repoName'].id" -o tsv)
az repos policy build create --blocking true --branch master --build-definition-id $pipelineId --display-name 'Build' --enabled true --manual-queue-only false --queue-on-source-update-only false --repository-id $repoId --valid-duration 0 --path-filter /src/Infrastructure/**/*
```
