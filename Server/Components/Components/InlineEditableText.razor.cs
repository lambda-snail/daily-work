using System.Net.WebSockets;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Server.Common;

namespace Server.Components.Components;

/// <summary>
/// A container for text that turns into an editable field or area when double-clicked.
/// </summary>
public partial class InlineEditableText: ComponentBase
{
    public enum InputMode
    {
        TextField,
        TextArea
    }

    private string _value = "";
    private string _originalValueWhenEditing = ""; 
    [Parameter] public string Value
    {
        get => _value;
        set
        {
            if (string.Equals(_value, value, StringComparison.InvariantCulture))
            {
                return;
            }

            _value = value;
            ValueChanged.InvokeAsync(value);
        }
    }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public InputMode Mode { get; set; } = InputMode.TextField;
    
    private bool _editMode = false;

    //[Parameter] public event EventHandler<string> ChangesSaved { get; set; }
    [Parameter] public EventCallback<string> ChangesSaved { get; set; }
    
    // protected override void OnInitialized()
    // {
    //     base.OnInitialized();
    //     _value.Text = Value;
    // }

    public void Update()
    {
        StateHasChanged();
    }
    
    private void OnDoubleClick(MouseEventArgs obj)
    {
        if (!_editMode)
        {
            _editMode = true;
            _originalValueWhenEditing = _value;
            StateHasChanged();
        }
    }

    private void CancelEditing()
    {
        _editMode = false;
        _value = _originalValueWhenEditing;
        StateHasChanged();
    }

    private void HandleValidSubmit()
    {
        if (!_editMode)
        {
            throw new InvalidOperationException("Submit is only valid when in edit state");
        }
        
        _editMode = false;
        if (_originalValueWhenEditing != _value)
        {
            ChangesSaved.InvokeAsync(Value);
        }
        
        StateHasChanged();
    }
}