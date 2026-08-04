@description('Identity database name.')
param name string

@description('Azure region for the resource.')
param location string

@description('Name of the parent Azure SQL logical server.')
param sqlServerName string

@description('Azure SQL database SKU name. The free offer requires the GP_S_Gen5 serverless SKU.')
param skuName string

@description('Azure SQL database SKU tier.')
param skuTier string

@description('Azure SQL database SKU family. Required for Gen5 SKUs.')
param skuFamily string = 'Gen5'

@description('Azure SQL database SKU capacity in vCores.')
param skuCapacity int

@description('What happens when the free monthly allowance is exhausted. AutoPause stops the database until the next month; BillOverUsage keeps it online and bills the overage.')
@allowed([
  'AutoPause'
  'BillOverUsage'
])
param freeLimitExhaustionBehavior string

@description('Maximum database size in bytes. The free offer caps data storage at 32 GB.')
param maxSizeBytes int

@description('Database collation.')
param collation string = 'SQL_Latin1_General_CP1_CI_AS'

@description('Common resource tags.')
param tags object

resource server 'Microsoft.Sql/servers@2023-08-01-preview' existing = {
  name: sqlServerName
}

resource database 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: server
  name: name
  location: location
  tags: tags
  sku: {
    name: skuName
    tier: skuTier
    family: skuFamily
    capacity: skuCapacity
  }
  properties: {
    collation: collation
    maxSizeBytes: maxSizeBytes
    zoneRedundant: false
    // The free offer includes its own backup storage, so
    // requestedBackupStorageRedundancy is deliberately not set here.
    useFreeLimit: true
    freeLimitExhaustionBehavior: freeLimitExhaustionBehavior
  }
}

output id string = database.id
output name string = database.name
