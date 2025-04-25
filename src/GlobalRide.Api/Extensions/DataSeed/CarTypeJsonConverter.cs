using System.Text.Json;
using System.Text.Json.Serialization;

using GlobalRide.Domain.CarTypes;

namespace GlobalRide.Api.Extensions.DataSeed;

/// <summary>
/// A custom JSON converter for the <see cref="CarType"/> type.
/// This converter handles serialization and deserialization of <see cref="CarType"/> objects to and from JSON.
/// </summary>
public class CarTypeJsonConverter : JsonConverter<CarType>
{
    /// <summary>
    /// Reads and converts JSON to a <see cref="CarType"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON from.</param>
    /// <param name="typeToConvert">The type to convert (in this case, <see cref="CarType"/>).</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use during deserialization.</param>
    /// <returns>A <see cref="CarType"/> object representing the deserialized JSON data.</returns>
    /// <exception cref="JsonException">
    /// Thrown when:
    /// - The JSON structure is invalid (e.g., missing required properties or unexpected tokens).
    /// - The <see cref="CarType.Create"/> method fails validation.
    /// </exception>
    public override CarType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object");
        }

        Guid? id = null;
        string? category = null;
        bool? isOneWayDropoffAllowed = null;
        decimal? oneWayFeeMultiplier = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected property name");
            }

            string propertyName = reader.GetString()!;
            reader.Read();

            switch (propertyName.ToLowerInvariant())
            {
                case "id":
                    id = reader.GetGuid();
                    break;
                case "category":
                    category = reader.GetString();
                    break;
                case "isonewaydropoffallowed":
                    isOneWayDropoffAllowed = reader.GetBoolean();
                    break;
                case "onewayfeemultiplier":
                    oneWayFeeMultiplier = reader.TokenType == JsonTokenType.Null ? null : reader.GetDecimal();
                    break;
            }
        }

        if (!id.HasValue)
        {
            throw new JsonException("Required property 'id' not found");
        }

        if (category == null)
        {
            throw new JsonException("Required property 'category' not found");
        }

        if (!isOneWayDropoffAllowed.HasValue)
        {
            throw new JsonException("Required property 'isOneWayDropoffAllowed' not found");
        }

        var result = CarType.Create(
            id.Value,
            category,
            isOneWayDropoffAllowed.Value,
            oneWayFeeMultiplier);

        return result.IsFailure
            ? throw new JsonException(result.Errors.Select(error => error.Message).Aggregate((a, b) => $"{a}\n{b}"))
            : result.Value;
    }

    /// <summary>
    /// Writes a <see cref="CarType"/> object to JSON.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON to.</param>
    /// <param name="value">The <see cref="CarType"/> object to serialize.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use during serialization.</param>
    public override void Write(Utf8JsonWriter writer, CarType value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("id");
        writer.WriteStringValue(value.Id);

        writer.WritePropertyName("category");
        writer.WriteStringValue(value.Category);

        writer.WritePropertyName("isOneWayDropoffAllowed");
        writer.WriteBooleanValue(value.IsOneWayDropoffAllowed);

        writer.WritePropertyName("oneWayFeeMultiplier");
        if (value.OneWayFeeMultiplier.HasValue)
        {
            writer.WriteNumberValue(value.OneWayFeeMultiplier.Value);
        }
        else
        {
            writer.WriteNullValue();
        }

        writer.WriteEndObject();
    }
}
