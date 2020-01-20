using System;
using System.Collections.Generic;

namespace EliteAPI.Events
{
    using Newtonsoft.Json;

    public class BountyInfo : IEvent
    {
        internal static BountyInfo Process(string json, EliteDangerousAPI api) => api.Events.InvokeBountyEvent(JsonConvert.DeserializeObject<BountyInfo>(json, JsonSettings.Settings));

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; internal set; }
        [JsonProperty("event")]
        public string Event { get; internal set; }
        [JsonProperty("Rewards")]
        public List<Reward> Rewards { get; internal set; }
        [JsonProperty("Target")]
        public string Target { get; internal set; }
        [JsonProperty("TotalReward")]
        public long TotalReward { get; internal set; }
        [JsonProperty("VictimFaction")]
        public string VictimFaction { get; internal set; }
        [JsonProperty("SharedWithOthers")]
        public long SharedWithOthers { get; internal set; }
    }
}
