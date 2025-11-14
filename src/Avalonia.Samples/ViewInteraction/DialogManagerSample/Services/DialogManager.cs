using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace DialogManagerSample.Services;

/// <summary>
/// The dialog manager helps to register an <see cref="IDialogParticipant"/> to a certain <see cref="Visual"/>
/// </summary>
public class DialogManager
{
    // this dictionary stores the mapping
    private static readonly Dictionary<IDialogParticipant, Visual> RegistrationMapper =
        new Dictionary<IDialogParticipant, Visual>();

    static DialogManager()
    {
        // add a listener to changes of the attached register property
        RegisterProperty.Changed.AddClassHandler<Visual>(RegisterChanged);
    }

    private static void RegisterChanged(Visual sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is null)
        {
            throw new InvalidOperationException("The DialogManager can only be registered on a Visual");
        }

        // Unregister any old registered context
        if (e.GetOldValue<IDialogParticipant>() is { } oldValue)
        {
            RegistrationMapper.Remove(oldValue);
        }

        // Register any new context
        if (e.GetNewValue<IDialogParticipant>() is { } newValue)
        {
            RegistrationMapper.Add(newValue, sender);
        }
    }

    /// <summary>
    /// This property handles the registration of Views and ViewModel
    /// </summary>
    public static readonly AttachedProperty<IDialogParticipant> RegisterProperty =
        AvaloniaProperty.RegisterAttached<DialogManager, Visual, IDialogParticipant>(
            "Register");

    /// <summary>
    /// Accessor for attached property <see cref="RegisterProperty"/>.
    /// </summary>
    public static void SetRegister(AvaloniaObject element, IDialogParticipant value)
    {
        element.SetValue(RegisterProperty, value);
    }

    /// <summary>
    /// Accessor for attached property <see cref="RegisterProperty"/>.
    /// </summary>
    public static IDialogParticipant GetRegister(AvaloniaObject element)
    {
        return element.GetValue(RegisterProperty);
    }

    /// <summary>
    /// Gets the associated <see cref="Visual"/> for a given context. Returns null, if none was registered
    /// </summary>
    /// <param name="context">The context to lookup</param>
    /// <returns>The registered Visual for the context or null if none was found</returns>
    public static Visual? GetVisualForContext(IDialogParticipant context)
    {
        return RegistrationMapper.GetValueOrDefault(context);
    }

    /// <summary>
    /// Gets the parent <see cref="TopLevel"/> for the given context. Returns null, if no TopLevel was found
    /// </summary>
    /// <param name="context">The context to lookup</param>
    /// <returns>The registered TopLevel for the context or null if none was found</returns>
    public static TopLevel? GetTopLevelForContext(IDialogParticipant context)
    {
        return TopLevel.GetTopLevel(GetVisualForContext(context));
    }
}