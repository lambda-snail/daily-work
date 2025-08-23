namespace Server.Features.Todo;

public class Todo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    public TodoState State { get; set; } = TodoState.New;
    
    public DateOnly Date { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public required Guid Owner { get; set; }
    
    public List<TodoItem> Items { get; set; } = new();

    /// <summary>
    /// Reevaluates the state of the todo.
    /// </summary>
    /// <returns>True if the state has changed.</returns>
    public bool EvaluateState()
    {
        var previousState = State;
        var completed = Items.Count(i => i.IsDone == true);
        if (completed > 0)
        {
            State = completed == Items.Count ? TodoState.Done : TodoState.InProgress;
        }
        else
        {
            State = TodoState.New;
        }

        return previousState != State;
    }
}