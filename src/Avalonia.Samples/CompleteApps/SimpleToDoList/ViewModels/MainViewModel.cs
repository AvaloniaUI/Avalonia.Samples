using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleToDoList.Models;
using SimpleToDoList.Services;

namespace SimpleToDoList.ViewModels;

/// <summary>
/// This is our MainViewModel in which we define the ViewModel logic to interact between our View and the TodoItems
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        // We can use this to add some items for the designer. 
        // You can also use a DesignTime-ViewModel
        if (Design.IsDesignMode)
        {
            ToDoItems = new ObservableCollection<ToDoItemViewModel>(new[]
            {
                new ToDoItemViewModel() { Content = "Hello" },
                new ToDoItemViewModel() { Content = "Avalonia", IsChecked = true}
            });
        }
    }
    
    /// <summary>
    /// Gets a collection of <see cref="ToDoItem"/> which allows adding and removing items
    /// </summary>
    public ObservableCollection<ToDoItemViewModel> ToDoItems { get; } = new ObservableCollection<ToDoItemViewModel>();

    
    // -- Adding new Items --
    
    /// <summary>
    /// This command is used to add a new Item to the List
    /// </summary>
    [RelayCommand (CanExecute = nameof(CanAddItem))]
    private void AddItem()
    {
        // Add a new item to the list
        ToDoItems.Add(new ToDoItemViewModel() {Content = NewItemContent});
        
        // reset the NewItemContent
        NewItemContent = null;
    }

    /// <summary>
    /// Gets or set the content for new Items to add. If this string is not empty, the AddItemCommand will be enabled automatically
    /// </summary>
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(AddItemCommand))] // This attribute will invalidate the command each time this property changes
    private string? _newItemContent;

    /// <summary>
    /// Returns if a new Item can be added. We require to have the NewItem some Text
    /// </summary>
    private bool CanAddItem() => !string.IsNullOrWhiteSpace(NewItemContent);
    
    // -- Removing Items --
    
    /// <summary>
    /// Removes the given Item from the list
    /// </summary>
    /// <param name="item">the item to remove</param>
    [RelayCommand]
    private void RemoveItem(ToDoItemViewModel item)
    {
        // Remove the given item from the list
        ToDoItems.Remove(item);
    }
}