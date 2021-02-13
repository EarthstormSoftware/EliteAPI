using System;

using EliteAPI.Event.Models;
using EliteAPI.Event.Models.Abstractions;

using Newtonsoft.Json;

using ProtoBuf;

namespace EliteAPI.Event.Models
{

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(500, typeof(EventBase))]
    public partial class PromotionEvent : EventBase
    {
        internal PromotionEvent() { }

        [JsonProperty("Empire")]
        public long Empire { get; private set; }
    }

    public partial class PromotionEvent
    {
        public static PromotionEvent FromJson(string json)
        {
            return JsonConvert.DeserializeObject<PromotionEvent>(json);
        }
    }
}

namespace EliteAPI.Event.Handler
{
    public partial class EventHandler
    {
        public event EventHandler<PromotionEvent> PromotionEvent;

        internal void InvokePromotionEvent(PromotionEvent arg)
        {
            PromotionEvent?.Invoke(this, arg);
        }
    }
}