using System;

using EliteAPI.Event.Models;
using EliteAPI.Event.Models.Abstractions;

using Newtonsoft.Json;

using ProtoBuf;

namespace EliteAPI.Event.Models
{

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(500, typeof(EventBase))]
    public partial class WingInviteEvent : EventBase
    {
        internal WingInviteEvent() { }

        [JsonProperty("Name")]
        public string Name { get; private set; }
    }

    public partial class WingInviteEvent
    {
        public static WingInviteEvent FromJson(string json)
        {
            return JsonConvert.DeserializeObject<WingInviteEvent>(json);
        }
    }
}

namespace EliteAPI.Event.Handler
{
    public partial class EventHandler
    {
        public event EventHandler<WingInviteEvent> WingInviteEvent;

        internal void InvokeWingInviteEvent(WingInviteEvent arg)
        {
            WingInviteEvent?.Invoke(this, arg);
        }
    }
}