using Radical.Servitizing.Event;
using System;

namespace Radical.Servitizing.DTO
{
    public class EventDTO : DTO
    {
        public virtual int EventVersion { get; set; }
        public virtual string EventType { get; set; }
        public virtual long AggregateId { get; set; }
        public virtual string AggregateType { get; set; }
        public virtual string EventData { get; set; }
        public virtual DateTime PublishTime { get; set; }
        public virtual EventPublishStatus PublishStatus { get; set; }
    }
}