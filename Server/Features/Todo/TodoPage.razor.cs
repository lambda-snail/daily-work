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
    private List<Item> _items = Enumerable.Range(1, 10).Select(i => new Item { Id = i, Name = $"Item {i}" }).ToList();
    

    public TodoPage(TodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
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

    private async Task AddTodo()
    {
        var todo = new Todo
        {
            Title = "New Todo",
            Description = "Describe your TODO here",
            Owner = 1
        };
        
        await _todoRepository.CreateTodo(todo);
    }
}