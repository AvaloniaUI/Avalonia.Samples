using System;
using Avalonia.Media;

namespace AdvancedToDoList.Helper;

public class ColorHelper
{
    public static Color GetRandomColor()
    {
        return new Color(
            255,
            (byte)Random.Shared.Next(255),
            (byte)Random.Shared.Next(255),
            (byte)Random.Shared.Next(255));
    }
}