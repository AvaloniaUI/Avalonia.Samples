using CommunityToolkit.Mvvm.Messaging;

namespace AdvancedToDoList.Messages;

/// <summary>
/// A class that represents a message for communication between different ViewModels.
/// It notifies about changed objects that should update.
/// </summary>
/// <param name="affectedItems">the updated items</param>
/// <param name="action">defines the type of the update (added, updated, removed)</param>
/// <typeparam name="T">the type of the updated items</typeparam>
/// <remarks>Used via <see cref="WeakReferenceMessenger"/>.</remarks>
public class UpdateDataMessage<T>(UpdateAction action, params T[] affectedItems)
{
    internal static void CreateAndSend(UpdateAction action, params T[] affectedItems)
    {
        WeakReferenceMessenger.Default.Send(new UpdateDataMessage<T>(action, affectedItems));
    }

    /// <summary>
    /// Gets the UpdateAction (added, updates, remove).
    /// </summary>
    public UpdateAction Action => action;
    
    /// <summary>
    /// Gets the items that were updated.
    /// </summary>
    public T[] ItemsAffected => affectedItems;
}