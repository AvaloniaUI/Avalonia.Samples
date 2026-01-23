using System;
using Avalonia.Media;

namespace AdvancedToDoList.Helper;

/// <summary>
/// An internal helper class for the <see cref="Avalonia.Media.Color"/>-struct
/// </summary>
internal static class ColorHelper
{
    /// <summary>
    /// Creates a Random Color using RGB
    /// </summary>
    /// <returns>The random Color</returns>
    internal static Color GetRandomColor()
    {
        return new Color(
            255,
            (byte)Random.Shared.Next(255),
            (byte)Random.Shared.Next(255),
            (byte)Random.Shared.Next(255));
    }
}