using CommunityToolkit.Mvvm.Messaging;

namespace AdvancedToDoList.Messages;

/// <summary>
/// A class that repsents a message for communication between different ViewModels.
/// It notifies about changed objects that should update.
/// </summary>
/// <param name="affectedItems">the updated items</param>
/// <typeparam name="T">the type of the updated items</typeparam>
/// <remarks>Used via <see cref="WeakReferenceMessenger"/>.</remarks>
public class UpdateDataRequest<T>(params T[] affectedItems)
{
    /// <summary>
    /// Gets the items that were updated.
    /// </summary>
    public T[] ItemsAffected => affectedItems;
}