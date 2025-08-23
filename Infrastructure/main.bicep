targetScope='subscription'

var location        = 'swedencentral' 
var commonRgName    = 'rg-ls-common'
var prodRgName      = 'rg-ls-dailywork-prod'

var appName = 'app-ls-dailywork-prod'

resource commonRg 'Microsoft.Resources/resourceGroups@2024-11-01' = {
  name: commonRgName
  location: location
}

resource prodRg 'Microsoft.Resources/resourceGroups@2024-11-01' = {
  name: prodRgName
  location: location
}

module identity './identity.bicep' = {
    name: 'identity'
    scope: prodRg
    params: {
        identityName: 'mi-ls-dailywork-prod'    
    }
}

module common './common.bicep' = {
  name: 'commonRgDeploy'
  scope: commonRg
  params: {
    configStoreName: 'ac-ls-common'
    appServicePlanName: 'asp-ls-linux'
    appName: appName
    identity: identity.outputs.identity
  }
}

module resources './resources.bicep' = {
  name: 'resources'
  scope: prodRg
  params: {
    storageAccountName: common.outputs.storageAccountName
    appServicePlanResourceId: common.outputs.appServicePlanResourceId
    identity: identity.outputs.identity
    identityResourceId: identity.outputs.identityResourceId
    appName: appName
    appConfigEndpoint: 'https://${common.outputs.appConfiguration.name}.azconfig.io'
  }
}