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

The deployment is sized to stay inside Azure's free allowances. The Container
App requests 0.25 vCPU and 0.5 GiB and scales to zero, drawing on the
Consumption plan's monthly free grant of 180,000 vCPU-seconds, 360,000
GiB-seconds and 2 million requests per subscription. The database uses the Azure
SQL free offer, which provides 100,000
vCore-seconds of compute plus 32 GB of data and backup storage per month on the
`GP_S_Gen5` serverless SKU. The free offer is the only
supported billing mode for this template: `requestedBackupStorageRedundancy` is
not set, because the free offer includes its own backup storage.
`SQL_DATABASE_FREE_LIMIT_EXHAUSTION_BEHAVIOR` decides what happens when the
allowance runs out: `AutoPause` stops the database until the next month,
`BillOverUsage` keeps it online and bills the overage.

The Container App uses a public GHCR image, so the template does not configure
registry credentials. Its `ConnectionStrings__NexusIdentity` setting contains a
passwordless Azure SQL connection string using
`Authentication=Active Directory Managed Identity`, where the `User Id` selects
the attached user-assigned identity directly. `AZURE_CLIENT_ID` is also set so
that any other `DefaultAzureCredential` consumer in the container resolves the
same identity.

The container declares readiness and liveness probes. Readiness targets
`/health/ready`, which includes the database health check, so ingress traffic is
withheld until SQL is reachable. Liveness targets `/health/live`, which
deliberately runs no checks: a transient SQL outage must not cause the
orchestrator to restart every otherwise-healthy replica. Both endpoints bypass
the service-key middleware.
The service key is stored as a Container Apps secret and exposed to the container
through a secret reference. Callers must send it in the
`X-Internal-Key` header; internal ingress is a network boundary, not an
authentication mechanism.

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
| `SQL_DATABASE_SKU_NAME` | Explicit database SKU name. `GP_S_Gen5` for the free offer. |
| `SQL_DATABASE_SKU_TIER` | Explicit database SKU tier. `GeneralPurpose` for the free offer. |
| `SQL_DATABASE_SKU_CAPACITY` | Explicit numeric SKU capacity in vCores. |
| `SQL_DATABASE_FREE_LIMIT_EXHAUSTION_BEHAVIOR` | `AutoPause` or `BillOverUsage`. |
| `SQL_PUBLIC_NETWORK_ACCESS` | Must be `Enabled` for the current topology. `Disabled` is rejected until the Container Apps environment has VNet integration and SQL has a private endpoint and private DNS. |
| `SQL_ALLOW_AZURE_SERVICES` | Explicitly `true` or `false`; `true` creates Azure SQL's `0.0.0.0` firewall rule. |
| `INTERNAL_SERVICE_KEY` | Secret of at least 32 characters that trusted internal callers send in the `X-Internal-Key` header. |

No default network-access or database-SKU decision is embedded in the template.
Those architecture choices must be selected per environment. The current
non-VNet-integrated Container Apps environment reaches the SQL public FQDN, so
the template deliberately rejects disabled public access rather than deploying
an unreachable application.

After exporting the required values, validate and deploy from the repository root:

```bash
az bicep build --file infra/identity/main.bicep
az deployment group validate \
  --name identity-validate-$(date -u +%Y%m%d%H%M%S) \
  --resource-group <existing-resource-group> \
  --parameters infra/identity/main.dev.bicepparam
az deployment group create \
  --name identity-deploy-$(date -u +%Y%m%d%H%M%S) \
  --resource-group <existing-resource-group> \
  --parameters infra/identity/main.dev.bicepparam
```

Explicit deployment names keep runs correlatable in the resource group's
deployment history; omitting `--name` yields an autogenerated name per run.

`.github/workflows/infra-validation.yml` runs `bicep build` and `bicep lint`
over this directory on every pull request that touches `infra/**`, so template
and parameter errors surface before a deployment is attempted. It reads the
non-sensitive values from Actions variables and `INTERNAL_SERVICE_KEY` from
Actions secrets, falling back to placeholders so the check still runs on pull
requests from forks, where secrets are not exposed.

## Required post-provisioning database grant

Provisioning the managed identity does not create a contained database user.
Before the Identity API can access its database, a later controlled deployment
step must connect as the configured Entra administrator, create a user for the
Identity managed identity, and grant only the runtime permissions the API needs.
That database operation is deliberately outside this infrastructure-only change.

The runtime identity must not receive schema-management permissions. Database
migrations will use a separate migration principal in the later deployment
pipeline.

## Known gaps

- No container image is published yet. `src/Services/Identity/NexusSupport.Identity.Api/Dockerfile`
  is still a placeholder, so `IDENTITY_CONTAINER_IMAGE` has nothing to point at
  until the image build and publish pipeline lands.
- Flipping `SQL_ALLOW_AZURE_SERVICES` from `true` to `false` does not by itself
  revoke access. `az deployment group create` uses incremental mode, which omits
  the conditional resource but does not delete the already-created
  `AllowAllWindowsAzureIps` rule. Delete it explicitly when turning the flag off
  (`az sql server firewall-rule delete --name AllowAllWindowsAzureIps ...`),
  until the deployment pipeline manages deletions.
- `SQL_ALLOW_AZURE_SERVICES=true` creates the `0.0.0.0` firewall rule, which
  admits traffic from any Azure tenant, not only this one. Microsoft Entra-only
  authentication is the actual access control, which makes this acceptable for
  development. Production is expected to move to a VNet-integrated Container Apps
  environment with a SQL private endpoint and drop the rule entirely.
