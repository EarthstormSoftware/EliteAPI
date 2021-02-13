using System;

using EliteAPI.Event.Models;
using EliteAPI.Event.Models.Abstractions;

using Newtonsoft.Json;

using ProtoBuf;

namespace EliteAPI.Event.Models
{

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(500, typeof(EventBase))]
    public partial class DockFighterEvent : EventBase
    {
        internal DockFighterEvent() { }

        [JsonProperty("ID")]
        public string Id { get; private set; }
    }

    public partial class DockFighterEvent
    {
        public static DockFighterEvent FromJson(string json)
        {
            return JsonConvert.DeserializeObject<DockFighterEvent>(json);
        }
    }
}

namespace EliteAPI.Event.Handler
{
    public partial class EventHandler
    {
        public event EventHandler<DockFighterEvent> DockFighterEvent;

        internal void InvokeDockFighterEvent(DockFighterEvent arg)
        {
            DockFighterEvent?.Invoke(this, arg);
        }
    }
}