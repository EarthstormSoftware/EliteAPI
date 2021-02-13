using System;

using EliteAPI.Event.Models;
using EliteAPI.Event.Models.Abstractions;

using Newtonsoft.Json;

using ProtoBuf;

namespace EliteAPI.Event.Models
{

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(500, typeof(EventBase))]
    public partial class ShipyardNewEvent : EventBase
    {
        internal ShipyardNewEvent() { }

        [JsonProperty("ShipType")]
        public string ShipType { get; private set; }

        [JsonProperty("ShipType_Localised")]
        public string ShipTypeLocalised { get; private set; }

        [JsonProperty("NewShipID")]
        public string NewShipId { get; private set; }
    }

    public partial class ShipyardNewEvent
    {
        public static ShipyardNewEvent FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ShipyardNewEvent>(json);
        }
    }
}

namespace EliteAPI.Event.Handler
{
    public partial class EventHandler
    {
        public event EventHandler<ShipyardNewEvent> ShipyardNewEvent;

        internal void InvokeShipyardNewEvent(ShipyardNewEvent arg)
        {
            ShipyardNewEvent?.Invoke(this, arg);
        }
    }
}