using Dapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Server.Common.Settings;

namespace Server.Features.User;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(IOptions<DatabaseSettings> settings)
    {
        SqlConnectionStringBuilder builder = new(settings.Value.ConnectionString);
        _connectionString = builder.ConnectionString;
    }

    public async Task<ApplicationUser?> GetByObjectId(Guid objectId)
    {
        await using SqlConnection connection = new(_connectionString);
        var result = await connection.QueryAsync<ApplicationUser?>(@"select * from [User] where ObjectId = @ObjectId;", new { ObjectId = objectId });
        return result.FirstOrDefault();
    }

    public async Task Create(ApplicationUser user)
    {
        if (user.ObjectId == Guid.Empty)
        {
            throw new ArgumentException("Unable to create user record: an object id should be provided from Entra");
        }
        
        await using SqlConnection connection = new(_connectionString);
        await connection.ExecuteAsync(
            @"insert into [User](ObjectId, Email, DisplayName) values (@ObjectId, @Email, @DisplayName)", 
            user
        );
    }
}