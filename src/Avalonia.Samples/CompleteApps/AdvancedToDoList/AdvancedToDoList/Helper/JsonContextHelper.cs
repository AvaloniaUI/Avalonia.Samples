using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdvancedToDoList.Models;
using AdvancedToDoList.Properties;
using Avalonia.Media;

namespace AdvancedToDoList.Helper;

[JsonSerializable(typeof(DataBaseDto))]
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

public class JsonColorConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Color.TryParse(reader.GetString(), out Color value) ? value : default;
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}