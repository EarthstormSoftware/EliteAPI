using System;

using EliteAPI.Event.Models;
using EliteAPI.Event.Models.Abstractions;

using Newtonsoft.Json;

using ProtoBuf;

namespace EliteAPI.Event.Models
{

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(500, typeof(EventBase))]
    public partial class ResurrectEvent : EventBase
    {
        internal ResurrectEvent() { }

        [JsonProperty("Option")]
        public string Option { get; private set; }

        [JsonProperty("Cost")]
        public long Cost { get; private set; }

        [JsonProperty("Bankrupt")]
        public bool Bankrupt { get; private set; }
    }

    public partial class ResurrectEvent
    {
        public static ResurrectEvent FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ResurrectEvent>(json);
        }
    }
}

namespace EliteAPI.Event.Handler
{
    public partial class EventHandler
    {
        public event EventHandler<ResurrectEvent> ResurrectEvent;

        internal void InvokeResurrectEvent(ResurrectEvent arg)
        {
            ResurrectEvent?.Invoke(this, arg);
        }
    }
}