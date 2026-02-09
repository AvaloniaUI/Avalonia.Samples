using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AdvancedToDoList.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// ViewModel for editing category details in a dialog window.
/// Handles form validation, save/cancel operations, and data persistence.
/// Works with the IDialogParticipant interface to show dialogs without UI dependencies.
/// </summary>
/// <remarks>
/// <para><b>What is EditCategoryViewModel?</b></para>
/// This ViewModel manages the user interface for creating or editing categories.
/// It handles:
/// - Displaying category properties (name, description, color) in a form
/// - Validating user input before saving
/// - Saving changes to the database
/// - Showing dialogs for errors or confirmation
/// - Notifying other parts of the app when data changes
/// 
/// <para><b>Why use a separate ViewModel for editing?</b></para>
/// - Separation of concerns: Business logic is separate from UI
/// - Testability: Can test logic without launching the UI
/// - Reusability: Same logic can work with different dialog implementations
/// - MVVM pattern: Follows Avalonia's recommended architecture
/// 
/// <para><b>How it works:</b></para>
/// 1. Created with a CategoryViewModel (existing or new)
/// 2. User edits properties in the dialog
/// 3. Save command validates and persists changes
/// 4. Cancel command can discard changes
/// 5. Result is returned to the calling ViewModel
/// 
/// <para><b>Key concepts:</b></para>
/// - RelayCommand: Automatically creates commands from methods
/// - IDialogParticipant: Enables dialog operations from ViewModels
/// - Service injection: Dependencies are passed in for testability
/// - WeakReferenceMessenger: Notifies other ViewModels of changes
/// </remarks>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
[UnconditionalSuppressMessage("Trimming", "IL2112", Justification = "We have all needed members added via DynamicallyAccessedMembers-Attribute")]
[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "We have all needed members added via DynamicallyAccessedMembers-Attribute")]
public partial class EditCategoryViewModel : ViewModelBase, IDialogParticipant
{
    /// <summary>
    /// Service for showing dialogs and notification messages.
    /// Injected to enable testing without real dialog implementation.
    /// </summary>
    /// <remarks>
    /// Why inject IDialogService instead of using it directly?
    /// - Allows mocking in unit tests
    /// - Decouples ViewModel from UI framework
    /// - Makes code more testable and flexible
    /// 
    /// The DialogService uses IDialogParticipant (this ViewModel) to:
    /// - Find the parent window for dialog positioning
    /// - Return results when dialogs close
    /// </remarks>
    private readonly IDialogService _dialogService;
    
    /// <summary>
    /// Service for persisting category data to the database.
    /// Handles save and delete operations for categories.
    /// </summary>
    /// <remarks>
    /// Why inject ICategoryService?
    /// - Enables dependency injection pattern
    /// - Allows unit testing with mock implementations
    /// - Decouples from specific database implementation
    /// 
    /// Operations provided:
    /// - SaveCategoryAsync: Insert or update a category
    /// - DeleteCategoryAsync: Remove a category (not used here)
    /// </remarks>
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// Parameterless constructor for design-time (Avalonia Designer).
    /// Creates a new empty category for design preview purposes.
    /// </summary>
    /// <remarks>
    /// Why have a parameterless constructor?
    /// - Avalonia Designer needs to create instances for preview
    /// - Provides default values for design surface
    /// - Avoids null reference in designer
    /// 
    /// Note: In production code, always use the full constructor
    /// with required services for proper functionality.
    /// </remarks>
    public EditCategoryViewModel() : this(new CategoryViewModel())
    {
    }
    
    /// <summary>
    /// Creates a new EditCategoryViewModel with default services.
    /// Convenience constructor for production use.
    /// </summary>
    /// <param name="category">The category to edit (existing or new)</param>
    /// <remarks>
    /// Why this overload?
    /// - Simplifies creation in most scenarios
    /// - Automatically resolves services from DI container
    /// - Falls back to default implementations if DI not available
    /// 
    /// Service resolution:
    /// - Tries to get ICategoryService from App.Services
    /// - Creates CategoryService if DI not available
    /// - Passes null for DialogService (uses default)
    /// </remarks>
    public EditCategoryViewModel(CategoryViewModel category) 
        : this(category, 
            App.Services.GetService<ICategoryService>() ?? new CategoryService(),
            null) // DialogService needs the participant (this), so we initialize it in the constructor body if null
    {
    }

    /// <summary>
    /// Primary constructor for creating EditCategoryViewModel with injected services.
    /// Full constructor for maximum flexibility and testability.
    /// </summary>
    /// <param name="category">The category to edit (existing to modify, new to create)</param>
    /// <param name="categoryService">Service for database operations on categories</param>
    /// <param name="dialogService">Service for showing dialogs and notifications (null = create new)</param>
    /// <remarks>
    /// Constructor chaining pattern:
    /// - Parameterless → this(new CategoryViewModel())
    /// - Single parameter → this(category, services, null)
    /// - Full constructor → Actual initialization
    /// 
    /// DialogService initialization:
    /// - If null provided, creates new DialogService(this)
    /// - "this" is the IDialogParticipant for finding parent window
    /// - Important: Must pass the ViewModel, not the View
    /// 
    /// For beginners: This pattern allows:
    /// - Easy creation (most common case: 1-param constructor)
    /// - Testing (2-param with mock services)
    /// - Full control (3-param with specific services)
    /// </remarks>
    public EditCategoryViewModel(CategoryViewModel category, ICategoryService categoryService, IDialogService? dialogService)
    {
        Item = category;
        _categoryService = categoryService;
        _dialogService = dialogService ?? new DialogService(this);
    }

    /// <summary>
    /// Gets the category being edited.
    /// </summary>
    /// <remarks>
    /// Why expose CategoryViewModel instead of Category model?
    /// - ViewModel has property change notifications
    /// - Better for data binding in XAML
    /// - Handles UI-specific logic (colors, validation)
    /// 
    /// The Item property is the "model" for this dialog.
    /// Changes are made to this object, then saved.
    /// 
    /// For editing existing category:
    /// - Pass the existing CategoryViewModel from the list
    /// - User changes properties
    /// - Save persists changes
    /// 
    /// For creating new category:
    /// - Pass new CategoryViewModel() with defaults
    /// - User fills in details
    /// - Save creates new database record
    /// </remarks>
    public CategoryViewModel Item { get; }
    
    /// <summary>
    /// Saves the edited category to the database.
    /// Validates input, persists changes, and returns result to caller.
    /// </summary>
    /// <remarks>
    /// <para><b>What this method does:</b></para>
    /// 1. Validates the category (required fields, etc.)
    /// 2. If valid, saves to database
    /// 3. If successful, notifies other ViewModels
    /// 4. Returns the saved category to the dialog caller
    /// 5. If unsuccessful, shows error dialog
    /// 
    /// <para><b>Validation flow:</b></para>
    /// - Calls Item.Validate() from ViewModelBase
    /// - Checks Item.HasErrors property
    /// - Shows error dialog if validation fails
    /// - Prevents saving invalid data
    /// 
    /// <para><b>Save process:</b></para>
    /// - Converts ViewModel back to Model (ToCategory())
    /// - Calls categoryService.SaveCategoryAsync()
    /// - Waits for database operation
    /// 
    /// <para><b>Success handling:</b></para>
    /// - Notifies other ViewModels via WeakReferenceMessenger
    /// - Creates new CategoryViewModel from saved Category
    /// - Returns result to dialog caller
    /// 
    /// <para><b>Error handling:</b></para>
    /// - Shows error dialog if save fails
    /// - User can try again or cancel
    /// 
    /// <para><b>RelayCommand magic:</b></para>
    /// [RelayCommand] attribute automatically:
    /// - Creates SaveCommand from this method
    /// - Handles async execution
    /// - Manages CanExecute state
    /// - Creates CancelCommand for async operations
    /// 
    /// <para><b>For beginners - Step by step:</b></para>
    /// 1. User clicks "Save" button in dialog
    /// 2. RelayCommand executes SaveAsync()
    /// 3. Validate() checks for errors (e.g., empty name)
    /// 4. If errors: Show dialog, return (dialog stays open)
    /// 5. If valid: Convert ViewModel → Model
    /// 6. Call service.SaveCategoryAsync()
    /// 7. If success:
    ///    - Send message to other ViewModels
    ///    - Return result (dialog closes)
    /// 8. If failure: Show error dialog
    /// </remarks>
    [RelayCommand]
    private async Task SaveAsync()
    {
        // Step 1: Validate user input
        // ViewModelBase.Validate() checks all [Required] attributes
        // and populates the Errors collection
        Item.Validate();
        
        // Check if validation found any errors
        if (Item.HasErrors)
        {
            // Show error dialog and exit (dialog stays open)
            await _dialogService.ShowOverlayDialogAsync<DialogResult>("Error", "Please correct the errors in the form.", DialogCommands.Ok);
            return;
        }
        
        // Step 2: Convert to model and save
        // ToCategory() extracts the underlying Category model
        // with the updated property values
        var category = Item.ToCategory();
        
        // Call the database service to persist changes
        // Returns true if saved successfully, false on error
        var success = await _categoryService.SaveCategoryAsync(category);

        // Step 3: Handle result
        if (success)
        {
            // Notify other ViewModels about the change
            // They can refresh their data if needed
            // TODO UpdateDataMessage<Category>.CreateAndSend(category);
            
            // Create a new ViewModel from the saved category
            // Note: Database may have assigned an ID to new categories
            _dialogService.ReturnResultFromOverlayDialog(new CategoryViewModel(category));
        }
        else
        {
            // Show an error dialog if save failed
            await _dialogService.ShowOverlayDialogAsync<DialogResult>("Error", "An error occurred while saving the category.", DialogCommands.Ok);
        }
    }

    /// <summary>
    /// Handles the cancel/dismiss action with optional save confirmation.
    /// Asks user if they want to save before closing.
    /// </summary>
    /// <remarks>
    /// <para><b>What this method does:</b></para>
    /// 1. Shows confirmation dialog with Yes/No/Cancel options
    /// 2. If Yes: Calls SaveAsync() then closes
    /// 3. If No: Returns null (discard changes) and closes
    /// 4. If Cancel: Returns (dialog stays open)
    /// 
    /// <para><b>User experience:</b></para>
    /// - User clicks cancel or closes dialog
    /// - Dialog asks: "Save changes before closing?"
    /// - User chooses: Yes (save), No (discard), Cancel (keep editing)
    /// 
    /// <para><b>Dialog flow:</b></para>
    /// - ShowOverlayDialogAsync blocks until user responds
    /// - Returns DialogResult (Yes, No, Cancel, or null)
    /// - Switch statement handles each case
    /// 
    /// <para><b>Return values:</b></para>
    /// - Yes → Calls SaveAsync(), returns saved category
    /// - No → Returns null (caller knows to discard)
    /// - Cancel → Returns (dialog stays open)
    /// 
    /// <para><b>For beginners - Understanding the flow:</b></para>
    /// 1. User clicks Cancel button or X button
    /// 2. RelayCommand executes CancelAsync()
    /// 3. Shows dialog: "Save changes? Yes/No/Cancel"
    /// 4. User responds:
    ///    - Yes: SaveAsync() runs → returns category
    ///    - No: Returns null → changes lost
    ///    - Cancel: Return → user can continue editing
    /// 
    /// <para><b>Design consideration:</b></para>
    /// Currently shows confirmation even for unchanged items.
    /// Enhancement: Track IsDirty flag to only ask if changes made.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Should never occur - indicates unexpected DialogResult value
    /// </exception>
    [RelayCommand]
    private async Task CancelAsync()
    {
        // Show confirmation dialog
        // Blocks until user clicks Yes, No, or Cancel
        var userResponse = await _dialogService.ShowOverlayDialogAsync<DialogResult>(
            "Save changes?", 
            "Do you want to save the changes before closing this dialog?", 
            DialogCommands.YesNoCancel);

        // Handle user's choice
        switch (userResponse)
        {
            case DialogResult.Yes:
                // Save and close (SaveAsync returns the result)
                await this.SaveAsync();
                break;
                
            case DialogResult.No:
                // Discard changes - return null to indicate cancellation
                _dialogService.ReturnResultFromOverlayDialog(null);
                break;
                
            case DialogResult.Cancel:
                // User changed mind - return without closing
                return;
                
            default:
                // Safety check - should never happen with YesNoCancel
                throw new ArgumentOutOfRangeException();
        }
    }
}