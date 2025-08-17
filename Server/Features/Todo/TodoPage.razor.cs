using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.FluentUI.AspNetCore.Components;
using Server.Common;
using Server.Common.Settings;

namespace Server.Features.Todo;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public bool Disabled { get; set; } = false;
}

[RenderModeInteractiveServer]
public partial class TodoPage : ComponentBase
{
    private readonly TodoRepository _todoRepository;
    
    private List<Todo> _items = new();
    private Todo? CurrentTodo { get; set; } = null;
    
    private FluentSortableList<TodoItem> _itemList;

    public TodoPage(TodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _items = await _todoRepository.GetTodos(1);
        StateHasChanged();
    }

    private void SortList(FluentSortableListEventArgs args)
    {
        if (args is null || args.OldIndex == args.NewIndex)
        {
            return;
        }

        var oldIndex = args.OldIndex;
        var newIndex = args.NewIndex;

        var items = this._items;
        var itemToMove = items[oldIndex];
        items.RemoveAt(oldIndex);

        if (newIndex < items.Count)
        {
            items.Insert(newIndex, itemToMove);
        }
        else
        {
            items.Add(itemToMove);
        }
    }

    private void SetCurrentTodo(Todo todo)
    {
        if (CurrentTodo != todo)
        {
            CurrentTodo = todo;
            StateHasChanged();
        }
    }
    
    private async Task AddTodo()
    {
        var todo = new Todo
        {
            Title = "New Todo",
            Description = "Describe your TODO here",
            Owner = 1,
            Items = new List<TodoItem>()
            {
                new TodoItem
                {
                    IsDone = false,
                    Text = "Do some task"
                }
            }
        };
        
        todo = await _todoRepository.CreateTodo(todo);
        _items.Add(todo);
        SetCurrentTodo(todo);
    }

    /// <summary>
    /// Manual binding to checkbox for todo item so we can do more than just update the visuals
    /// </summary>
    private async Task CheckboxValueChanged(bool value, int itemId)
    {
        var item = CurrentTodo?.Items.Where(i => i.Id == itemId)?.FirstOrDefault();
     
        ArgumentNullException.ThrowIfNull(CurrentTodo);
        ArgumentNullException.ThrowIfNull(item);
        
        item.IsDone = value;
        await _todoRepository.UpdateTodoItem(item, CurrentTodo);
    }

    private async Task AddItemToCurrentTodo()
    {
        ArgumentNullException.ThrowIfNull(CurrentTodo);

        var item = new TodoItem
        {
            Text = "Describe your task or goal",
            ParentTodo = CurrentTodo.Id
        };
        
        item = await _todoRepository.CreateTodoItem(item);
        CurrentTodo.Items.Add(item);
        StateHasChanged();
    }

    private string GetDateTimeDescriptionString(DateTime dt)
    {
        var timePassed = DateTime.Now - dt;
        return timePassed switch
        {
            { TotalSeconds: < 60 } => $"{(int)timePassed.TotalSeconds} seconds ago",
            { TotalMinutes: < 30 } => $"{(int)timePassed.TotalMinutes} minutes ago",
            { TotalMinutes: >= 30 and < 90 } => $"about an hour ago",
            { TotalHours: < 6 } => $"{(int)timePassed.TotalHours} hours ago",
            { TotalDays: 1 } => $"one day ago",
            { TotalDays: < 7 } => $"{(int)timePassed.TotalDays} days ago",
            _ => dt.ToString("yyyy-MM-dd HH:mm")
        };
    }
}