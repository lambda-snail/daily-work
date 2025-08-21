using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Server.Common.Settings;

namespace Server.Features.Todo;

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

        foreach (var item in todo.Items)
        {
            item.ParentTodo = todo.Id;
        }
        
        if (todo.Items.Count > 0)
        {
            await connection.ExecuteAsync(
                @"insert into TodoItem(IsDone, Text, ParentTodo) values (@IsDone, @Text, @ParentTodo)", 
                todo.Items
            );
        }
        
        var result = await connection.QueryAsync<Todo, TodoItem, Todo>(
            @"select * from Todo 
                    inner join TodoItem on TodoItem.ParentTodo = Todo.Id
                    where Todo.Id = @Id;",
            (t, i) =>
            {
                t.Items.Add(i);
                return t;
            }, 
            new { Id = todo.Id });
        
        return result.First();
    }

    public async Task UpdateTodoItem(TodoItem item, Todo todo)
    {
        await using SqlConnection connection = new(_connectionString);
        
        // For simplicity, we update everything - for now
        await connection.ExecuteAsync(
            @"update TodoItem set IsDone = @IsDone, Text = @Text where Id = @Id", 
            new { Id = item.Id, IsDone = item.IsDone, Text = item.Text }
        );
        
        todo.LastUpdated = DateTime.Now;
        await connection.ExecuteAsync(
            @"update Todo set LastUpdated = @LastUpdated where Id = @Id", 
            todo
        );
    }
    
    public async Task<TodoItem> CreateTodoItem(TodoItem item)
    {
        await using SqlConnection connection = new(_connectionString);
        
        item.Id = await connection.QuerySingleAsync<int>(
            @"insert into TodoItem(IsDone, Text, ParentTodo) values (@IsDone, @Text, @ParentTodo); select cast(SCOPE_IDENTITY() as int)", 
            item
        );

        return item;
    }

    public async Task<List<Todo>> GetTodosForUser(int userId)
    {
        await using SqlConnection connection = new(_connectionString);
        Dictionary<int, Todo> todos = new();
        var result = await connection.QueryAsync<Todo, TodoItem, Todo>(
            @"select * from Todo 
                    inner join TodoItem on TodoItem.ParentTodo = Todo.Id
                    where Todo.Owner = @Owner;",
            (t, i) =>
            {
                if (!todos.TryGetValue(t.Id, out Todo? todo))
                {
                    todos.Add(t.Id, t);
                    todo = t;
                }
                
                todo.Items.Add(i);
                return todo;
            }, 
            new { Owner = userId });

        return todos.Values.ToList();
    }
    
    public async Task<List<Todo>> GetTodosForUser(int userId, DateOnly yearMonthFilter)
    {
        await using SqlConnection connection = new(_connectionString);
        Dictionary<int, Todo> todos = new();
        var result = await connection.QueryAsync<Todo, TodoItem, Todo>(
            @"select * from Todo 
                    inner join TodoItem on TodoItem.ParentTodo = Todo.Id
                    where Todo.Owner = @Owner and YEAR(Todo.Date) = @Year and MONTH(Todo.Date) = @Month",
            (t, i) =>
            {
                if (!todos.TryGetValue(t.Id, out Todo? todo))
                {
                    todos.Add(t.Id, t);
                    todo = t;
                }
                
                todo.Items.Add(i);
                return todo;
            }, 
            new { Owner = userId, Year =  yearMonthFilter.Year, Month = yearMonthFilter.Month });

        return todos.Values.ToList();
    }

    /// <summary>
    /// Updates a todo but does not propagate the update to the items.
    /// </summary>
    public async Task UpdateTodoNonCascading(Todo todo)
    {
        await using SqlConnection connection = new(_connectionString);
        
        todo.LastUpdated = DateTime.Now;
        await connection.ExecuteAsync(
            @"update Todo set Title = @Title, Description = @Description, LastUpdated = @LastUpdated where Id = @Id", 
            todo
        );
    }

    public async Task DeleteTodo(Todo todo)
    {
        await using SqlConnection connection = new(_connectionString);
        
        // Cascading rules will ensure items are deleted as well
        await connection.ExecuteAsync(
            @"delete from [Todo] where Id = @Id",
            todo
        );
    }
}