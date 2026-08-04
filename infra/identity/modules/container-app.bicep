@description('Identity Container App name.')
param name string

@description('Azure region for the resource.')
param location string

@description('Resource ID of the shared Container Apps environment.')
param containerAppsEnvironmentId string

@description('Resource ID of the Identity user-assigned managed identity.')
param managedIdentityId string

@description('Client ID of the Identity user-assigned managed identity.')
param managedIdentityClientId string

@description('Public GHCR Identity image including a tag or digest.')
param containerImage string

@description('Identity API container port.')
param targetPort int

@description('Minimum replica count.')
param minReplicas int

@description('Maximum replica count.')
param maxReplicas int

@description('Azure SQL logical server fully qualified domain name.')
param sqlServerFullyQualifiedDomainName string

@description('Identity database name.')
param databaseName string

@secure()
@description('Shared secret used to authenticate callers of the Identity API.')
param identityServiceKey string

@description('Common resource tags.')
param tags object

var identityConnectionString = 'Server=tcp:${sqlServerFullyQualifiedDomainName},1433;Initial Catalog=${databaseName};Authentication=Active Directory Default;User Id=${managedIdentityClientId};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: name
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
  properties: {
    environmentId: containerAppsEnvironmentId
    configuration: {
      activeRevisionsMode: 'Single'
      secrets: [
        {
          name: 'identity-service-key'
          value: identityServiceKey
        }
      ]
      ingress: {
        external: false
        allowInsecure: false
        targetPort: targetPort
        transport: 'auto'
      }
    }
    template: {
      containers: [
        {
          name: 'identity-api'
          image: containerImage
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
            }
            {
              name: 'AZURE_CLIENT_ID'
              value: managedIdentityClientId
            }
            {
              name: 'ConnectionStrings__NexusIdentity'
              value: identityConnectionString
            }
            {
              name: 'IdentityServiceKey'
              secretRef: 'identity-service-key'
            }
          ]
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

output id string = containerApp.id
output name string = containerApp.name
