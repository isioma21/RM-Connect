using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RmConnect.IntegrationTests;

public static class HttpExtensions
{
    // The API sends enums as text ("Pending", "Call"), so reading needs the same setting
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task<T> ReadAsync<T>(this HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<T>(Json))!;
}
