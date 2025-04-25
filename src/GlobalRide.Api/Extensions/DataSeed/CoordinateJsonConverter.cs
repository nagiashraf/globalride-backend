using System.Text.Json;
using System.Text.Json.Serialization;

using GlobalRide.Domain.Branches;

namespace GlobalRide.Api.Extensions.DataSeed;

/// <summary>
/// A custom JSON converter for the <see cref="Coordinate"/> type.
/// This converter handles serialization and deserialization of <see cref="Coordinate"/> objects to and from JSON.
/// </summary>
public class CoordinateJsonConverter : JsonConverter<Coordinate>
{
    /// <summary>
    /// Reads and converts JSON to a <see cref="Coordinate"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON from.</param>
    /// <param name="typeToConvert">The type to convert (in this case, <see cref="Coordinate"/>).</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use during deserialization.</param>
    /// <returns>A <see cref="Coordinate"/> object representing the deserialized JSON data.</returns>
    /// <exception cref="JsonException">Thrown when the JSON data is invalid or cannot be converted to a <see cref="Coordinate"/>.</exception>
    public override Coordinate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        var latitude = root.GetProperty("latitude").GetDouble();
        var longitude = root.GetProperty("longitude").GetDouble();

        var result = Coordinate.Create(latitude, longitude);
        return result.IsSuccess
            ? result.Value
            : throw new JsonException(result.Errors.Select(error => error.Message).Aggregate((a, b) => $"{a}\n{b}"));
    }

    /// <summary>
    /// Writes a <see cref="Coordinate"/> object to JSON.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON to.</param>
    /// <param name="value">The <see cref="Coordinate"/> object to serialize.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use during serialization.</param>
    public override void Write(Utf8JsonWriter writer, Coordinate value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("latitude", value.Latitude);
        writer.WriteNumber("longitude", value.Longitude);
        writer.WriteEndObject();
    }
}
