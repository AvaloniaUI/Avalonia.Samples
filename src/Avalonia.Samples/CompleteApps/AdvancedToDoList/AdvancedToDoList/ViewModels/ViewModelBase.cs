using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedToDoList.ViewModels;

[UnconditionalSuppressMessage("Trimming", "IL2026: Using member 'CommunityToolkit.Mvvm.ComponentModel.ObservableValidator.ObservableValidator()' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. The type of the current instance cannot be statically discovered.",
    Justification = "Handled via rd.xml")]
public abstract class ViewModelBase : ObservableValidator
{
    public void Validate() => ValidateAllProperties();
}
