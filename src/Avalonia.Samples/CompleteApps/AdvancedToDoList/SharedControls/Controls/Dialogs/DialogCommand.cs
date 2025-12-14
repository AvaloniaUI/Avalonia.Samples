using System.Windows.Input;

namespace SharedControls.Controls;

public class DialogCommand : ICommand
{
    /// <summary>
    /// Gets or sets the caption of the button.
    /// </summary>
    public required object Caption { get; init; }

    /// <summary>
    /// Gets or sets the command to execute when the button is clicked.
    /// If the command is null, the <see cref="DialogResult"/> property will be returned.
    /// </summary>
    public ICommand? Command
    {
        get => field ?? this;
        init;
    }

    internal Action<object?>? InternalCommandExecutor;

    /// <summary>
    /// Gets or sets the result to return when the button is clicked. This property is used as the CommandParameter of the Command property.
    /// </summary>
    public required object? DialogResult { get; init; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the button is the default button.
    /// </summary>
    public bool IsDefault { get; init; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the button is the cancel button.
    /// </summary>
    public bool IsCancel { get; init; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (Command != this)
        {
            Command?.Execute(parameter);
        }
        else
        {
            InternalCommandExecutor?.Invoke(DialogResult);
        }
    }

    public event EventHandler? CanExecuteChanged;
}

public static class DialogCommands 
{
    /// <summary>
    /// Gets the default cancel button.
    /// </summary>
    public static DialogCommand Cancel { get; } = new DialogCommand
    {
        Caption = "Cancel", 
        IsCancel = true, 
        DialogResult = DialogResult.Cancel
    };
    
    /// <summary>
    /// Gets the default OK button.
    /// </summary>
    public static DialogCommand Ok { get; } = new DialogCommand
    {
        Caption = "OK", 
        IsDefault = true, 
        DialogResult = DialogResult.Cancel
    };
    
    /// <summary>
    /// Gets the default Yes button.
    /// </summary>
    public static DialogCommand Yes { get; } = new DialogCommand
    {
        Caption = "Yes", 
        IsDefault = true,
        DialogResult = DialogResult.Yes
    };

    /// <summary>
    /// Gets the default No button.
    /// </summary>
    public static DialogCommand No { get; } = new DialogCommand
    {
        Caption = "No",
        DialogResult = DialogResult.No
    };

    /// <summary>
    /// Gets the default OK and Cancel buttons.
    /// </summary>
    public static DialogCommand[] OkCancel = [Cancel, Ok];

    /// <summary>
    /// Gets the default OK button.
    /// </summary>
    public static DialogCommand[] OkOnly = [Ok];

    /// <summary>
    /// Gets the default Yes, No, and Cancel buttons.
    /// </summary>
    public static DialogCommand[] YesNoCancel = [Cancel, No, Yes];
}