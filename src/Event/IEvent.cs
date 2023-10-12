using System;

namespace Radical.Servitizing.Event;

using Entity;

public interface IEvent : IEntity
{
    uint EventVersion { get; set; }
    string EventType { get; set; }
    byte[] EventData { get; set; }
    long AggregateId { get; set; }
    string AggregateType { get; set; }
    DateTime PublishTime { get; set; }
    EventPublishStatus PublishStatus { get; set; }
}