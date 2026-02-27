namespace AdvancedToDoList.Messages;

public enum UpdateAction
{
    /// <summary>
    /// The items provided were added
    /// </summary>
    Added,
    
    /// <summary>
    /// The items provided were updated 
    /// </summary>
    Updated,
    
    /// <summary>
    /// The items provided were removed
    /// </summary>
    Removed, 
    
    /// <summary>
    /// The collection has bulk changes and should be reset
    /// </summary>
    Reset
}