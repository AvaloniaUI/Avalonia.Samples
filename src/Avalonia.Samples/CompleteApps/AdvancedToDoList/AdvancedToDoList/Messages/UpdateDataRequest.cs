using System;

namespace AdvancedToDoList.Messages;

public class UpdateDataRequest<T>(params T[] affectedItems)
{
    public T[] ItemsAffected => affectedItems;
}