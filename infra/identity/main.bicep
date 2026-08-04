targetScope = 'resourceGroup'

@description('Short application name used in Azure resource names.')
@minLength(2)
param applicationName string = 'nexus-support'

@description('Deployment environment name, for example dev, staging, or prod.')
@minLength(2)
param environmentName string

@description('Azure region supplied by the deployment workflow from GitHub repository configuration.')
param location string

@description('Suffix used to make globally unique resource names. Use only lowercase letters and numbers.')
@minLength(3)
param resourceNameSuffix string

@description('Public GHCR image including an immutable tag or digest.')
param containerImage string

@description('Microsoft Entra tenant ID used by the Azure SQL administrator.')
param entraTenantId string

@description('Display name of the Microsoft Entra principal used as the Azure SQL administrator.')
param sqlEntraAdministratorLogin string

@description('Object ID of the Microsoft Entra principal used as the Azure SQL administrator.')
param sqlEntraAdministratorObjectId string

@description('Principal type of the Microsoft Entra SQL administrator.')
@allowed([
  'User'
  'Group'
  'Application'
])
param sqlEntraAdministratorPrincipalType string

@description('Azure SQL database SKU name, supplied explicitly per environment.')
param sqlDatabaseSkuName string

@description('Azure SQL database SKU tier, supplied explicitly per environment.')
param sqlDatabaseSkuTier string

@description('Azure SQL database SKU capacity, supplied explicitly per environment.')
@minValue(0)
param sqlDatabaseSkuCapacity int

@description('Controls whether the Azure SQL logical server accepts public network traffic.')
@allowed([
  'Enabled'
  'Disabled'
])
param sqlPublicNetworkAccess string

@description('Explicitly allows Azure-hosted resources to reach SQL through its public endpoint. This creates the 0.0.0.0 firewall rule.')
param allowAzureServicesToAccessSql bool

@description('Container port exposed by the Identity API image.')
@minValue(1)
@maxValue(65535)
param identityTargetPort int = 8080

@description('Minimum number of Identity replicas. Zero preserves scale-to-zero on the Consumption plan.')
@minValue(0)
param identityMinReplicas int = 0

@description('Maximum number of Identity replicas.')
@minValue(1)
param identityMaxReplicas int = 1

var normalizedApplicationName = toLower(applicationName)
var normalizedEnvironmentName = toLower(environmentName)
var normalizedSuffix = toLower(resourceNameSuffix)
var baseName = '${normalizedApplicationName}-${normalizedEnvironmentName}'
var containerAppsEnvironmentName = 'cae-${baseName}'
var identityName = 'ca-identity-${normalizedEnvironmentName}'
var identityManagedIdentityName = 'id-identity-${normalizedEnvironmentName}'
var sqlServerName = 'sql-${normalizedApplicationName}-${normalizedEnvironmentName}-${normalizedSuffix}'
var identityDatabaseName = 'sqldb-identity-${normalizedEnvironmentName}'

module containerAppsEnvironment 'modules/container-apps-environment.bicep' = {
  name: 'container-apps-environment'
  params: {
    name: containerAppsEnvironmentName
    location: location
    tags: sharedTags
  }
}

module identityManagedIdentity 'modules/managed-identity.bicep' = {
  name: 'identity-managed-identity'
  params: {
    name: identityManagedIdentityName
    location: location
    tags: identityTags
  }
}

module sqlServer 'modules/sql-server.bicep' = {
  name: 'shared-sql-server'
  params: {
    name: sqlServerName
    location: location
    entraTenantId: entraTenantId
    entraAdministratorLogin: sqlEntraAdministratorLogin
    entraAdministratorObjectId: sqlEntraAdministratorObjectId
    entraAdministratorPrincipalType: sqlEntraAdministratorPrincipalType
    publicNetworkAccess: sqlPublicNetworkAccess
    allowAzureServices: allowAzureServicesToAccessSql
    tags: sharedTags
  }
}

module identityDatabase 'modules/database.bicep' = {
  name: 'identity-database'
  params: {
    name: identityDatabaseName
    location: location
    sqlServerName: sqlServer.outputs.name
    skuName: sqlDatabaseSkuName
    skuTier: sqlDatabaseSkuTier
    skuCapacity: sqlDatabaseSkuCapacity
    tags: identityTags
  }
}

module identityContainerApp 'modules/container-app.bicep' = {
  name: 'identity-container-app'
  params: {
    name: identityName
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    managedIdentityId: identityManagedIdentity.outputs.id
    managedIdentityClientId: identityManagedIdentity.outputs.clientId
    containerImage: containerImage
    targetPort: identityTargetPort
    minReplicas: identityMinReplicas
    maxReplicas: identityMaxReplicas
    sqlServerFullyQualifiedDomainName: sqlServer.outputs.fullyQualifiedDomainName
    databaseName: identityDatabase.outputs.name
    tags: identityTags
  }
}

var commonTags = {
  application: normalizedApplicationName
  environment: normalizedEnvironmentName
  'managed-by': 'bicep'
}

var sharedTags = union(commonTags, {
  scope: 'shared'
})

var identityTags = union(commonTags, {
  service: 'identity'
})

output containerAppsEnvironmentId string = containerAppsEnvironment.outputs.id
output identityContainerAppId string = identityContainerApp.outputs.id
output identityContainerAppName string = identityContainerApp.outputs.name
output identityManagedIdentityClientId string = identityManagedIdentity.outputs.clientId
output identityDatabaseName string = identityDatabase.outputs.name
output sqlServerFullyQualifiedDomainName string = sqlServer.outputs.fullyQualifiedDomainName
