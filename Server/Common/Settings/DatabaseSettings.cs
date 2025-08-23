namespace Server.Common.Settings;

public class DatabaseSettings
{
    /// <summary>
    /// For using managed identity with connection string, see https://learn.microsoft.com/en-us/sql/connect/ado-net/sql/azure-active-directory-authentication?view=sql-server-ver16#using-managed-identity-authentication
    /// </summary>
    public required string ConnectionString { get; set; }
}