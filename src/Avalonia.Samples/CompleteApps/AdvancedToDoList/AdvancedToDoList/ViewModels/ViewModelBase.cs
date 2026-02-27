using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// Base class for all ViewModels in the application.
/// Inherits from ObservableValidator to provide data validation capabilities
/// and ensure only valid data is saved to the database.
/// </summary>
[UnconditionalSuppressMessage("Trimming", "IL2112", Justification = "We have all needed members added via DynamicallyAccessedMembers-Attribute")]
[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "We have all needed members added via DynamicallyAccessedMembers-Attribute")]
public abstract class ViewModelBase : ObservableValidator
{
    /// <summary>
    /// Validates all properties of the ViewModel using data annotations.
    /// This method should be called before saving data to ensure validity.
    /// Populates the validation errors that can be accessed through HasErrors property.
    /// </summary>
    public void Validate() => ValidateAllProperties();
}
