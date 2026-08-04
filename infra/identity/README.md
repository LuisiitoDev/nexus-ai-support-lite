# Identity infrastructure

This directory contains resource-group-scoped Bicep for the Identity service and
the shared resources it needs initially. The resource group itself is created
outside this deployment.

## Resources

The entry point provisions:

- a shared Azure Container Apps Consumption environment;
- a shared Azure SQL logical server configured for Microsoft Entra-only authentication;
- an Identity-owned Azure SQL database;
- an Identity user-assigned managed identity; and
- an internally exposed Identity Container App that can scale to zero.

The shared environment uses the Consumption plan and has no Log Analytics
workspace configured. A telemetry destination remains an explicit future
architecture decision rather than being provisioned implicitly here.

The Container App uses a public GHCR image, so the template does not configure
registry credentials. Its `ConnectionStrings__NexusIdentity` setting contains a
passwordless Azure SQL connection string. `AZURE_CLIENT_ID` selects the attached
user-assigned identity for `Active Directory Default` authentication.

## Deployment inputs

`main.bicep` intentionally has no default for environment-specific or unresolved
infrastructure decisions. `main.dev.bicepparam` reads those values from process
environment variables so values managed through the
`LuisiitoDev/nexus-ai-support-lite` GitHub repository configuration do not need to
be committed.

| Environment variable | Purpose |
| --- | --- |
| `AZURE_LOCATION` | Azure region, currently intended to be `eastus2`. |
| `AZURE_RESOURCE_NAME_SUFFIX` | Lowercase alphanumeric suffix for the globally unique SQL server name. |
| `IDENTITY_CONTAINER_IMAGE` | Public `ghcr.io` image with an immutable tag or digest. |
| `AZURE_TENANT_ID` | Microsoft Entra tenant containing the SQL administrator. |
| `SQL_ENTRA_ADMINISTRATOR_LOGIN` | Display/login name for the Entra administrator. |
| `SQL_ENTRA_ADMINISTRATOR_OBJECT_ID` | Object ID for the Entra administrator. |
| `SQL_ENTRA_ADMINISTRATOR_PRINCIPAL_TYPE` | `User`, `Group`, or `Application`. |
| `SQL_DATABASE_SKU_NAME` | Explicit database SKU name for the environment. |
| `SQL_DATABASE_SKU_TIER` | Explicit database SKU tier for the environment. |
| `SQL_DATABASE_SKU_CAPACITY` | Explicit numeric SKU capacity for the environment. |
| `SQL_PUBLIC_NETWORK_ACCESS` | Explicitly `Enabled` or `Disabled`. |
| `SQL_ALLOW_AZURE_SERVICES` | Explicitly `true` or `false`; `true` creates Azure SQL's `0.0.0.0` firewall rule. |

No default network-access or database-SKU decision is embedded in the template.
Those architecture choices must be selected per environment.

After exporting the required values, validate and deploy from the repository root:

```bash
az bicep build --file infra/identity/main.bicep
az deployment group validate \
  --resource-group <existing-resource-group> \
  --parameters infra/identity/main.dev.bicepparam
az deployment group create \
  --resource-group <existing-resource-group> \
  --parameters infra/identity/main.dev.bicepparam
```

## Required post-provisioning database grant

Provisioning the managed identity does not create a contained database user.
Before the Identity API can access its database, a later controlled deployment
step must connect as the configured Entra administrator, create a user for the
Identity managed identity, and grant only the runtime permissions the API needs.
That database operation is deliberately outside this infrastructure-only change.

The runtime identity must not receive schema-management permissions. Database
migrations will use a separate migration principal in the later deployment
pipeline.
