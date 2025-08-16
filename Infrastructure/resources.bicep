param identity object

param identityResourceId string
param appServicePlanResourceId string
param storageAccountName string

param appConfigEndpoint string
param appName string

// param logAnalyticsName string = 'log-ls-dailywork-prod'
// param appInsightsName string = 'appi-ls-dailywork-prod'

//var monitoringMetricsPublisherId = '3913510d-42f4-4e42-8a64-420c390055eb'

// resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
//   name: logAnalyticsName
//   location: resourceGroup().location
//   properties: any({
//     retentionInDays: 10
//     features: {
//       searchVersion: 1
//     }
//     sku: {
//       name: 'Free'
//     }
//   })
// }

// resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
//   name: appInsightsName
//   location: resourceGroup().location
//   kind: 'web'
//   properties: {
//     Application_Type: 'web'
//     WorkspaceResourceId: logAnalytics.id
//     DisableLocalAuth: true
//   }
// }

// resource roleAssignmentAppInsights 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
//   name: guid(subscription().id, applicationInsights.id, identity.properties.clientId, 'Monitoring Metrics Publisher')
//   scope: applicationInsights
//   properties: {
//     roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', monitoringMetricsPublisherId)
//     principalId: identity.properties.principalId
//     principalType: 'ServicePrincipal'
//   }
// }




resource webApp 'Microsoft.Web/sites@2024-04-01' = {
  name: appName
  location: resourceGroup().location
  kind: 'app,linux'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identityResourceId}':{}
      }
    }
  properties: {
    serverFarmId: appServicePlanResourceId
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
      webSocketsEnabled: true
      linuxFxVersion: 'DOTNETCORE|9.0'
    }
  }
  resource configAppSettings 'config' = {
    name: 'appsettings'
    properties: {
        AzureWebJobsStorage__accountName: storageAccountName
        AzureWebJobsStorage__credential : 'managedidentity'
        AzureWebJobsStorage__clientId: identity.properties.clientId
        ClientId: identity.properties.clientId
        //AppConfigurationEndpoint: app
        //APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsights.properties.InstrumentationKey
        //APPLICATIONINSIGHTS_AUTHENTICATION_STRING: 'ClientId=${identity.properties.clientId};Authorization=AAD'
      }
  }
}