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
param internalServiceKey string

@description('Common resource tags.')
param tags object

// 'Active Directory Managed Identity' with an explicit User Id selects the
// attached user-assigned identity directly. 'Active Directory Default' would
// delegate to DefaultAzureCredential, where User Id is not the selector and
// combining the two has produced ambiguous authentication behaviour.
var identityConnectionString = 'Server=tcp:${sqlServerFullyQualifiedDomainName},1433;Initial Catalog=${databaseName};Authentication=Active Directory Managed Identity;User Id=${managedIdentityClientId};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

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
          name: 'internal-service-key'
          value: internalServiceKey
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
              name: 'InternalServiceKey'
              secretRef: 'internal-service-key'
            }
          ]
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          probes: [
            {
              type: 'Readiness'
              httpGet: {
                path: '/health/ready'
                port: targetPort
                scheme: 'HTTP'
              }
              initialDelaySeconds: 5
              periodSeconds: 10
              timeoutSeconds: 5
              failureThreshold: 3
            }
            {
              type: 'Liveness'
              httpGet: {
                path: '/health/live'
                port: targetPort
                scheme: 'HTTP'
              }
              initialDelaySeconds: 30
              periodSeconds: 30
              timeoutSeconds: 5
              failureThreshold: 3
            }
          ]
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
