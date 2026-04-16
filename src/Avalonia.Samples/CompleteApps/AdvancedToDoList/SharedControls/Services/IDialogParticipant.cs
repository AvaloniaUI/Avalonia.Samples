namespace SharedControls.Services;

/// <summary>
/// Marker interface that identifies a class as capable of participating in dialog operations.
/// Used to associate ViewModels (or other objects) with their corresponding Views/Windows.
/// 
/// Why use this interface?
/// - Enables ViewModels to show dialogs without direct UI framework dependencies
/// - Provides clean separation between business logic and UI
/// - Makes ViewModels more testable (can mock dialog operations)
/// - Works with DialogHelper extension methods for easy dialog access
/// 
/// How it works:
/// 1. Implement this interface on your ViewModel or class
/// 2. Register the association using DialogManager.RegisterProperty in XAML
/// 3. Use DialogHelper extension methods to show dialogs
/// 4. DialogHelper will automatically find the associated View/Window
/// </summary>
/// <example>
/// In ViewModel:
/// <code language="csharp">
/// <![CDATA[
/// public class MyViewModel : ViewModelBase, IDialogParticipant
/// {
///     public async Task ShowMyDialog()
///     {
///         var result = await this.ShowOverlayDialogAsync<string>(
///             "My Dialog",
///             "Dialog content",
///             DialogCommands.OkCancel);
///     }
/// }
/// ]]>
/// </code>
/// 
/// In XAML View:
/// <code language="xaml">
/// <![CDATA[
/// <UserControl local:DialogManager.Register="{Binding}" />
/// ]]>
/// </code>
/// </example>
public interface IDialogParticipant
{
    // Empty marker interface - no methods needed
    // The implementation and association is handled by DialogManager
}