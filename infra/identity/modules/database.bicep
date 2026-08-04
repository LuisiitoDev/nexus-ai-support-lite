@description('Identity database name.')
param name string

@description('Azure region for the resource.')
param location string

@description('Name of the parent Azure SQL logical server.')
param sqlServerName string

@description('Azure SQL database SKU name.')
param skuName string

@description('Azure SQL database SKU tier.')
param skuTier string

@description('Azure SQL database SKU capacity.')
param skuCapacity int

@description('Backup storage redundancy. Geo-redundant storage is billed at a premium and is not warranted for non-production environments.')
@allowed([
  'Local'
  'Zone'
  'Geo'
])
param backupStorageRedundancy string

@description('Maximum database size in bytes.')
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
    capacity: skuCapacity
  }
  properties: {
    collation: collation
    maxSizeBytes: maxSizeBytes
    requestedBackupStorageRedundancy: backupStorageRedundancy
  }
}

output id string = database.id
output name string = database.name
