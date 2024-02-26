using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleToDoList.Models;

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
            ToDoItems = new ObservableCollection<ToDoItem>(new[]
            {
                new ToDoItem() { Content = "Hello" },
                new ToDoItem() { Content = "Avalonia", IsChecked = true}
            });
        }
    }
    
    /// <summary>
    /// Gets a collection of <see cref="ToDoItem"/> which allows adding and removing items
    /// </summary>
    public ObservableCollection<ToDoItem> ToDoItems { get; } = new ObservableCollection<ToDoItem>();

    
    // -- Adding new Items --
    
    /// <summary>
    /// This command is used to add a new Item to the List
    /// </summary>
    /// <param name="content"></param>
    [RelayCommand (CanExecute = nameof(CanAddItem))]
    private void AddItem(string content)
    {
        // Add a new item to the list
        ToDoItems.Add(new ToDoItem() {Content = NewItemContent});
        
        // reset the NewItemContent
        NewItemContent = null;
    }

    /// <summary>
    /// Gets or set the content for new Items to add. If this string is not empty, the AddItemCommand will be enabled automatically
    /// </summary>
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(AddItemCommand))]
    private string? _NewItemContent;

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
    private void RemoveItem(ToDoItem item)
    {
        // Add a new item to the list
        ToDoItems.Remove(item);
    }
}