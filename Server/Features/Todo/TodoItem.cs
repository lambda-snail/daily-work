namespace Server.Features.Todo;

public class TodoItem
{
    public int Id { get; set; }
    public bool IsDone { get; set; } = false;
    public required string Text { get; set; }
    public int ParentTodo { get; set; }
}