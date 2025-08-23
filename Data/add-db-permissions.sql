-- Adds a managed identity as a db user and grants data reader and writer roles
-- See: https://learn.microsoft.com/en-us/azure/search/search-index-azure-sql-managed-instance-with-managed-identity

CREATE USER [managed-identity-name] FROM EXTERNAL PROVIDER;
EXEC sp_addrolemember 'db_datareader', [managed-identity-name];
EXEC sp_addrolemember 'db_datawriter', [managed-identity-name];