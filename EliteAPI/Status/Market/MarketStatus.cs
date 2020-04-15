﻿using System;
using System.Collections.Generic;
using System.Text;
using EliteAPI.Events;
using Newtonsoft.Json;

namespace EliteAPI.Status.Market
{
    public class MarketStatus : StatusBase
    {
        internal MarketStatus() { }

        [JsonProperty("MarketID")]
        public long MarketId { get; internal set; }

        [JsonProperty("StationName")]
        public string StationName { get; internal set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; internal set; }

        [JsonProperty("Items")]
        public IReadOnlyList<Item> Items { get; internal set; }

        internal override StatusBase Default => new MarketStatus();
    }
}
