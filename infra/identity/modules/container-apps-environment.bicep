@description('Azure Container Apps environment name.')
param name string

@description('Azure region for the resource.')
param location string

@description('Common resource tags.')
param tags object

resource environment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: name
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'none'
    }
    zoneRedundant: false
  }
}

output id string = environment.id
output name string = environment.name
