using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Server.Common.Settings;

namespace Server.Features.Todo;

public class TodoItem
{
    public int Id { get; set; }
    public bool IsDone { get; set; } = false;
    public required string Text { get; set; }
    public int ParentTodo { get; set; }
}

public class Todo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    public int Owner { get; set; }
    
    public List<TodoItem> Items { get; set; } = new();
}

public class TodoRepository
{
    private readonly string _connectionString;

    public TodoRepository(IOptions<DatabaseSettings> settings)
    {
        SqlConnectionStringBuilder builder =
            new(settings.Value.ConnectionString);

        _connectionString = builder.ConnectionString;
    }

    public async Task<Todo> CreateTodo(Todo todo)
    {
        await using SqlConnection connection = new(_connectionString);
        
        todo.Id = await connection.QuerySingleAsync<int>(
            @"insert into Todo(Title, Description, Owner) values (@Title, @Description, @Owner); select cast(SCOPE_IDENTITY() as int)", 
            todo
        );

        if (todo.Items.Count > 0)
        {
            var result = await connection.QueryAsync<int>(
                @"insert into TodoItem(IsDone, Text, ParentTodo) values (@IsDone, @Text, @ParentTodo); select cast(SCOPE_IDENTITY() as int)", 
                todo.Items
            );

            int index = 0;
            foreach (var id in result)
            {
                todo.Items[index++].Id = id;
            }
        }

        return todo;
    }
}