using System;

using EliteAPI.Event.Models;
using EliteAPI.Event.Models.Abstractions;

using Newtonsoft.Json;

using ProtoBuf;

namespace EliteAPI.Event.Models
{

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(500, typeof(EventBase))]
    public partial class ModuleBuyEvent : EventBase
    {
        internal ModuleBuyEvent() { }

        [JsonProperty("Slot")]
        public string Slot { get; private set; }

        [JsonProperty("BuyItem")]
        public string BuyItem { get; private set; }

        [JsonProperty("BuyItem_Localised")]
        public string BuyItemLocalised { get; private set; }

        [JsonProperty("MarketID")]
        public string MarketId { get; private set; }

        [JsonProperty("BuyPrice")]
        public long BuyPrice { get; private set; }

        [JsonProperty("Ship")]
        public string Ship { get; private set; }

        [JsonProperty("ShipID")]
        public string ShipId { get; private set; }
    }

    public partial class ModuleBuyEvent
    {
        public static ModuleBuyEvent FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ModuleBuyEvent>(json);
        }
    }
}

namespace EliteAPI.Event.Handler
{
    public partial class EventHandler
    {
        public event EventHandler<ModuleBuyEvent> ModuleBuyEvent;

        internal void InvokeModuleBuyEvent(ModuleBuyEvent arg)
        {
            ModuleBuyEvent?.Invoke(this, arg);
        }
    }
}