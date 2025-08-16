using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Server.Components.Components;

internal class Value
{
    public string? Text { get; set; }
}

public partial class InlineEditableText: ComponentBase
{
    public enum InputMode
    {
        TextField,
        TextArea
    }
    
    [Parameter] public string Value { get; set; } = "";
    [Parameter] public InputMode Mode { get; set; } = InputMode.TextField;
    
    private readonly Value _value = new();
    private bool _editMode = false;

    public event EventHandler<string> OnTextChanged;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _value.Text = Value;
    }

    private void OnDoubleClick(MouseEventArgs obj)
    {
        if (!_editMode)
        {
            _editMode = true;
            StateHasChanged();
        }
    }

    private void CancelEditing()
    {
        _editMode = false;
        _value.Text = Value;
        StateHasChanged();
    }

    private void HandleValidSubmit(EditContext obj)
    {
        if (!_editMode)
        {
            throw new InvalidOperationException("Submit is only valid when in edit state");
        }
        
        _editMode = false;
        if (Value != _value.Text)
        {
            Value = _value.Text ?? "";
            OnTextChanged?.Invoke(this, Value);
        }
        
        StateHasChanged();
    }
}