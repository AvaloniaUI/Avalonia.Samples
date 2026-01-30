using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// This is the base class for all of our ViewModels. It inherits <see cref="ObservableValidator"/> which helps us to
/// ensure only valid data is saved. 
/// </summary>
[UnconditionalSuppressMessage("Trimming", "IL2026: Using member 'CommunityToolkit.Mvvm.ComponentModel.ObservableValidator.ObservableValidator()' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. The type of the current instance cannot be statically discovered.",
    Justification = "Handled via rd.xml")]
public abstract class ViewModelBase : ObservableValidator
{
    public void Validate() => ValidateAllProperties();
}
