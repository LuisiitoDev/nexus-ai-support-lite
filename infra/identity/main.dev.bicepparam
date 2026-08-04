using './main.bicep'

// GitHub Actions will source environment-specific values from the repository's
// configuration and expose them to the Bicep deployment process.
param applicationName = 'nexus-support'
param environmentName = 'dev'
param location = readEnvironmentVariable('AZURE_LOCATION')
param resourceNameSuffix = readEnvironmentVariable('AZURE_RESOURCE_NAME_SUFFIX')
param containerImage = readEnvironmentVariable('IDENTITY_CONTAINER_IMAGE')
param entraTenantId = readEnvironmentVariable('AZURE_TENANT_ID')
param sqlEntraAdministratorLogin = readEnvironmentVariable('SQL_ENTRA_ADMINISTRATOR_LOGIN')
param sqlEntraAdministratorObjectId = readEnvironmentVariable('SQL_ENTRA_ADMINISTRATOR_OBJECT_ID')
param sqlEntraAdministratorPrincipalType = readEnvironmentVariable('SQL_ENTRA_ADMINISTRATOR_PRINCIPAL_TYPE')
param sqlDatabaseSkuName = readEnvironmentVariable('SQL_DATABASE_SKU_NAME')
param sqlDatabaseSkuTier = readEnvironmentVariable('SQL_DATABASE_SKU_TIER')
param sqlDatabaseSkuCapacity = int(readEnvironmentVariable('SQL_DATABASE_SKU_CAPACITY'))
param sqlPublicNetworkAccess = readEnvironmentVariable('SQL_PUBLIC_NETWORK_ACCESS')
param allowAzureServicesToAccessSql = bool(readEnvironmentVariable('SQL_ALLOW_AZURE_SERVICES'))
param identityServiceKey = readEnvironmentVariable('IDENTITY_SERVICE_KEY')
