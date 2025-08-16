param identityName string

resource appIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: identityName
  location: resourceGroup().location
}

output identityResourceId string = appIdentity.id
output identity object = appIdentity