using System.Text.Json.Serialization;

namespace RealtorApi.Infrastructure.Persistence;

public sealed class SeedManifestModel
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("statusesFile")]
    public string StatusesFile { get; set; } = string.Empty;

    [JsonPropertyName("propertiesFile")]
    public string PropertiesFile { get; set; } = string.Empty;

    [JsonPropertyName("imagesSourceDirectory")]
    public string ImagesSourceDirectory { get; set; } = string.Empty;

    [JsonPropertyName("imagesTargetDirectory")]
    public string ImagesTargetDirectory { get; set; } = string.Empty;

    [JsonPropertyName("publicImageBasePath")]
    public string PublicImageBasePath { get; set; } = string.Empty;
}

public sealed class PropertyStatusSeedModel
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public sealed class PropertySeedModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("bedroomCount")]
    public int BedroomCount { get; set; }

    [JsonPropertyName("bathroomCount")]
    public int BathroomCount { get; set; }

    [JsonPropertyName("areaSquareMeters")]
    public decimal AreaSquareMeters { get; set; }

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
