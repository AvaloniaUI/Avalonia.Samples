using System;
using Avalonia.Media;

namespace AdvancedToDoList.Helper;

/// <summary>
/// Internal helper class for generating and working with Avalonia Color objects.
/// Provides utility functions for color manipulation and random color generation.
/// Used primarily for creating default colors for new categories.
/// </summary>
internal static class ColorHelper
{
    /// <summary>
    /// Generates a fully opaque color (alpha = 255) with random RGB values.
    /// Used to provide visual distinction for newly created categories.
    /// </summary>
    /// <returns>A new random Color with full opacity</returns>
    internal static Color GetRandomColor()
    {
        return new Color(
            255, // Alpha channel (fully opaque)
            (byte)Random.Shared.Next(255), // Red channel (0-255)
            (byte)Random.Shared.Next(255), // Green channel (0-255)
            (byte)Random.Shared.Next(255)); // Blue channel (0-255)
    }
}