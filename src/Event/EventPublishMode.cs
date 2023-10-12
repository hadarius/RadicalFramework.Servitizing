using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Radical.Servitizing.Event
{
    public enum EventPublishMode
    {
        PropagateCommand,
        SuppressCommand
    }
}