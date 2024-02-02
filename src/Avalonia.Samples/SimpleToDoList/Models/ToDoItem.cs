using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleToDoList.Models;

/*   NOTE:
 * 
 *   Please Mind that this samples uses the CommunityToolkit.Mvvm package for the Model. Feel free to use any other
 *   MVVM-Framework (like ReactiveUI or Prsim) that suits your needs best.
 */

/// <summary>
/// This is our Model for a simple ToDoItem. 
/// </summary>
public partial class ToDoItem : ObservableObject 
{
    /// <summary>
    /// Gets or sets the checked status of each item
    /// </summary>
    [ObservableProperty] 
    private bool _IsChecked;

    /// <summary>
    /// Gets or sets the content of the to-do item
    /// </summary>
    /// <returns></returns>
    [ObservableProperty] 
    private string? _Content;
}