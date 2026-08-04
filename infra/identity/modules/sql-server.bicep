@description('Globally unique Azure SQL logical server name.')
param name string

@description('Azure region for the resource.')
param location string

@description('Microsoft Entra tenant ID.')
param entraTenantId string

@description('Display name of the Microsoft Entra administrator principal.')
param entraAdministratorLogin string

@description('Object ID of the Microsoft Entra administrator principal.')
param entraAdministratorObjectId string

@description('Type of the Microsoft Entra administrator principal.')
param entraAdministratorPrincipalType string

@description('Whether the SQL public network endpoint is enabled.')
param publicNetworkAccess string

@description('Whether to create the Azure-services firewall rule.')
param allowAzureServices bool

@description('Common resource tags.')
param tags object

resource server 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: name
  location: location
  tags: tags
  properties: {
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      login: entraAdministratorLogin
      principalType: entraAdministratorPrincipalType
      sid: entraAdministratorObjectId
      tenantId: entraTenantId
    }
    minimalTlsVersion: '1.2'
    publicNetworkAccess: publicNetworkAccess
    restrictOutboundNetworkAccess: 'Disabled'
  }
}

resource allowAzureRule 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = if (allowAzureServices) {
  parent: server
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output id string = server.id
output name string = server.name
output fullyQualifiedDomainName string = server.properties.fullyQualifiedDomainName
