using System;

using EliteAPI.Event.Models;
using EliteAPI.Event.Models.Abstractions;

using Newtonsoft.Json;

using ProtoBuf;

namespace EliteAPI.Event.Models
{

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(500, typeof(EventBase))]
    public partial class CrewAssignEvent : EventBase
    {
        internal CrewAssignEvent() { }

        [JsonProperty("Name")]
        public string Name { get; private set; }

        [JsonProperty("CrewID")]
        public string CrewId { get; private set; }

        [JsonProperty("Role")]
        public string Role { get; private set; }
    }

    public partial class CrewAssignEvent
    {
        public static CrewAssignEvent FromJson(string json)
        {
            return JsonConvert.DeserializeObject<CrewAssignEvent>(json);
        }
    }
}

namespace EliteAPI.Event.Handler
{
    public partial class EventHandler
    {
        public event EventHandler<CrewAssignEvent> CrewAssignEvent;

        internal void InvokeCrewAssignEvent(CrewAssignEvent arg)
        {
            CrewAssignEvent?.Invoke(this, arg);
        }
    }
}