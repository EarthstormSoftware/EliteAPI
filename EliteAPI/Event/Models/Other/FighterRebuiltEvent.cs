using System;

using EliteAPI.Event.Models;
using EliteAPI.Event.Models.Abstractions;

using Newtonsoft.Json;

using ProtoBuf;

namespace EliteAPI.Event.Models
{

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(500, typeof(EventBase))]
    public partial class FighterRebuiltEvent : EventBase
    {
        internal FighterRebuiltEvent() { }

        [JsonProperty("Loadout")]
        public string Loadout { get; private set; }

        [JsonProperty("ID")]
        public string Id { get; private set; }
    }

    public partial class FighterRebuiltEvent
    {
        public static FighterRebuiltEvent FromJson(string json)
        {
            return JsonConvert.DeserializeObject<FighterRebuiltEvent>(json);
        }
    }
}

namespace EliteAPI.Event.Handler
{
    public partial class EventHandler
    {
        public event EventHandler<FighterRebuiltEvent> FighterRebuiltEvent;

        internal void InvokeFighterRebuiltEvent(FighterRebuiltEvent arg)
        {
            FighterRebuiltEvent?.Invoke(this, arg);
        }
    }
}