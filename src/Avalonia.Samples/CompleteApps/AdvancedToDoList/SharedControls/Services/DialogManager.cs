using Avalonia;
using Avalonia.Controls;

namespace SharedControls.Services;

/// <summary>
/// Manages registration and lookup of dialog participants (typically ViewModels).
/// Provides the infrastructure for connecting ViewModels to their corresponding UI elements.
/// Uses attached properties to establish the ViewModel-to-View relationship.
/// </summary>
/// <remarks>
/// How DialogManager works:
/// 
/// 1. Registration:
///    - ViewModels implement IDialogParticipant interface
///    - XAML Views use DialogManager.RegisterProperty to bind to ViewModel
///    - This creates a mapping between ViewModel and View
/// 
/// 2. Lookup:
///    - When dialogs need to be shown, DialogHelper queries DialogManager
///    - DialogManager provides the associated Visual/Window for the ViewModel
///    - The dialog is then shown using that Visual as the parent
/// 
/// 3. Cleanup:
///    - Automatically removes registrations when Views detach from visual tree
///    - Prevents memory leaks from stale ViewModel-View associations
/// 
/// Benefits:
/// - ViewModels don't need references to UI controls
/// - Supports any type of dialog (overlay, modal window, etc.)
/// - Clean separation of concerns between MVVM layers
/// - Automatic resource management
/// </remarks>
/// <example>
/// Example ViewModel:
/// <code>
/// public class MyViewModel : ViewModelBase, IDialogParticipant
/// {
///     private async Task ShowDialogAsync()
///     {
///         await this.ShowOverlayDialogAsync&lt;string&gt;("Title", "Content");
///     }
/// }
/// </code>
/// 
/// Example XAML View:
/// <code>
/// &lt;UserControl x:Class="MyApp.Views.MyView"
///               local:DialogManager.Register="{Binding}"&gt;
///     &lt;!-- View content goes here --&gt;
/// &lt;/UserControl&gt;
/// </code>
/// </example>
public class DialogManager
{
    /// <summary>
    /// Internal dictionary that maps ViewModels (IDialogParticipant) to their Visual elements.
    /// Key: The ViewModel or any object implementing IDialogParticipant
    /// Value: The Visual control (View, Window, etc.) associated with the ViewModel
    /// </summary>
    private static readonly Dictionary<IDialogParticipant, Visual> RegistrationMapper =
        new Dictionary<IDialogParticipant, Visual>();

    /// <summary>
    /// Static constructor that initializes the registration system.
    /// Sets up event listener for the RegisterProperty attached property.
    /// Called automatically when DialogManager is first used.
    /// </summary>
    static DialogManager()
    {
        // Listen for changes to the RegisterProperty attached property
        // When a View sets this property, RegisterChanged is called
        RegisterProperty.Changed.AddClassHandler<Visual>(RegisterChanged);
    }

    /// <summary>
    /// Handles changes to the RegisterProperty attached property.
    /// Called when a View binds to a ViewModel or when that binding changes.
    /// </summary>
    /// <param name="sender">The Visual control that has the RegisterProperty</param>
    /// <param name="e">Event arguments containing old and new IDialogParticipant values</param>
    /// <exception cref="InvalidOperationException">Thrown if sender is not a Visual</exception>
    private static void RegisterChanged(Visual sender, AvaloniaPropertyChangedEventArgs e)
    {
        // Validate that sender is a Visual (should always be true)
        if (sender is null)
        {
            throw new InvalidOperationException("The DialogManager can only be registered on a Visual");
        }

        // Unregister any previously registered ViewModel
        // This happens when binding changes or View is reused
        if (e.GetOldValue<IDialogParticipant>() is { } oldValue)
        {
            RegistrationMapper.Remove(oldValue);
        }

        // Register the new ViewModel (if any)
        // This establishes the ViewModel-to-View mapping
        if (e.GetNewValue<IDialogParticipant>() is { } newValue)
        {
            RegistrationMapper.Add(newValue, sender);
            
            // Clean up mapping when View is removed from visual tree
            // Prevents memory leaks from dangling registrations
            // TODO: Known issue - DetachedFromVisualTree may not fire in all scenarios
            // Consider adding explicit cleanup in View's Unloaded event
            sender.DetachedFromVisualTree += (_, _) => RegistrationMapper.Remove(newValue);
        }
    }

    /// <summary>
    /// Attached property used to bind Views to their ViewModels.
    /// Set this property in XAML to register the ViewModel-View relationship.
    /// Typically bound to the ViewModel's DataContext or DataContext property.
    /// </summary>
    /// <remarks>
    /// In XAML, you would typically write:
    /// <code>
    /// &lt;UserControl local:DialogManager.Register="{Binding}"&gt;
    /// </code>
    /// This binds the RegisterProperty to the current ViewModel (DataContext).
    /// </remarks>
    public static readonly AttachedProperty<IDialogParticipant> RegisterProperty =
        AvaloniaProperty.RegisterAttached<DialogManager, Visual, IDialogParticipant>(
            "Register");

    /// <summary>
    /// Sets the RegisterProperty attached property on an element.
    /// Called by XAML binding engine or manually in code.
    /// </summary>
    /// <param name="element">The Visual control to register the ViewModel on</param>
    /// <param name="value">The ViewModel implementing IDialogParticipant</param>
    public static void SetRegister(AvaloniaObject element, IDialogParticipant value)
    {
        element.SetValue(RegisterProperty, value);
    }

    /// <summary>
    /// Gets the RegisterProperty attached property from an element.
    /// Used internally to retrieve the associated ViewModel.
    /// </summary>
    /// <param name="element">The Visual control to get the registered ViewModel from</param>
    /// <returns>The registered ViewModel or null if none is registered</returns>
    public static IDialogParticipant GetRegister(AvaloniaObject element)
    {
        return element.GetValue(RegisterProperty);
    }

    /// <summary>
    /// Looks up the Visual element associated with a dialog participant.
    /// Used by DialogHelper to find the UI element for a given ViewModel.
    /// </summary>
    /// <param name="context">The ViewModel or object implementing IDialogParticipant</param>
    /// <returns>The associated Visual element, or null if no registration found</returns>
    /// <example>
    /// <code>
    /// var viewModel = new MyViewModel();
    /// var visual = DialogManager.GetVisualForContext(viewModel);
    /// // visual will be the View/Window that registered this ViewModel
    /// </code>
    /// </example>
    public static Visual? GetVisualForContext(IDialogParticipant context)
    {
        return RegistrationMapper.GetValueOrDefault(context);
    }

    /// <summary>
    /// Finds the TopLevel (Window or equivalent) for a dialog participant.
    /// Used to determine the parent window for showing dialogs.
    /// </summary>
    /// <param name="context">The ViewModel or object implementing IDialogParticipant</param>
    /// <returns>The associated TopLevel window, or null if no registration found</returns>
    /// <example>
    /// <code>
    /// var viewModel = new MyViewModel();
    /// var window = DialogManager.GetTopLevelForContext(viewModel);
    /// // window will be the MainWindow or dialog containing the View
    /// </code>
    /// </example>
    public static TopLevel? GetTopLevelForContext(IDialogParticipant context)
    {
        // Get the Visual, then find its TopLevel parent
        return TopLevel.GetTopLevel(GetVisualForContext(context));
    }
}
