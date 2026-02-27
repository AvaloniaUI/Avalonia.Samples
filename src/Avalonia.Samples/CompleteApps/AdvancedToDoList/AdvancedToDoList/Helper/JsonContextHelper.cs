using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdvancedToDoList.Models;
using AdvancedToDoList.Properties;
using Avalonia.Media;

namespace AdvancedToDoList.Helper;

/// <summary>
/// JSON serialization context that enables AOT-compatible and trimming-friendly JSON operations.
/// Uses source generation to improve performance and reduce application size.
/// Essential for publishing to platforms that use ahead-of-time compilation or aggressive trimming.
/// </summary>
/// <see href="https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation"/>
[JsonSerializable(typeof(DatabaseDto))]
[JsonSerializable(typeof(Settings))]
[JsonSerializable(typeof(Category[]))]
[JsonSerializable(typeof(ToDoItem[]))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(Color))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class JsonContextHelper : JsonSerializerContext
{
}

/// <summary>
/// Custom JSON converter for Avalonia Color objects.
/// Handles serialization and deserialization of Color values as strings.
/// Converts between Color objects and their string representations for JSON storage.
/// </summary>
public class JsonColorConverter : JsonConverter<Color>
{
    /// <summary>
    /// Deserializes a string value from JSON into a Color object.
    /// Returns the default color if parsing fails, ensuring robust error handling.
    /// </summary>
    /// <param name="reader">The JSON reader to read from</param>
    /// <param name="typeToConvert">The type being converted (Color)</param>
    /// <param name="options">Serialization options</param>
    /// <returns>The deserialized Color object or default if parsing fails</returns>
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Color.TryParse(reader.GetString(), out Color value) ? value : default;
    }

    /// <summary>
    /// Serializes a Color object into a string for JSON output.
    /// Uses Color's built-in ToString() method for consistent formatting.
    /// </summary>
    /// <param name="writer">The JSON writer to write to</param>
    /// <param name="value">The Color object to serialize</param>
    /// <param name="options">Serialization options</param>
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}