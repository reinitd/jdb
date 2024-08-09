using Newtonsoft.Json;

namespace JDB;

[JsonObject]
public class Table {
    [JsonProperty("last_updated")]
    [JsonRequired]
    public long LastUpdated { get; set; }
    [JsonProperty("created_at")]
    [JsonRequired]
    public long CreatedAt { get; set; }
    [JsonProperty("data")]
    public Dictionary<string, object>? Data { get; set; }
}