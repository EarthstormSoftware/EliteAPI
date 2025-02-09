using EliteAPI.Abstractions.Events;
using Newtonsoft.Json;

namespace EliteAPI.Events;

public readonly struct DockingRequestedEvent : IEvent
{
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; init; }

    [JsonProperty("event")]
    public string Event { get; init; }

    [JsonProperty("MarketID")]
    public string MarketId { get; init; }

    [JsonProperty("StationName")]
    public string StationName { get; init; }

    [JsonProperty("StationType")]
    public string StationType { get; init; }

    [JsonProperty("LandingPads")]
    public LandingPadsInfo LandingPads { get; init; }


    public readonly struct LandingPadsInfo
    {
        [JsonProperty("Small")]
        public int Small { get; init; }

        [JsonProperty("Medium")]
        public int Medium { get; init; }

        [JsonProperty("Large")]
        public int Large { get; init; }
    }
}