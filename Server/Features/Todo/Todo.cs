namespace Server.Features.Todo;

public class Todo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    public DateOnly Date { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public required Guid Owner { get; set; }
    
    public List<TodoItem> Items { get; set; } = new();
}