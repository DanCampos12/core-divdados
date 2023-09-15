using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Results;
using System;
using System.Collections.Generic;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface IEventRepository
{
    Event GetEvent(Guid id, Guid userId);
    IEnumerable<EventResult> GetEvents(Guid userId);
    EventResult Add(Event @event);
    EventResult Update(Event @event);
    Guid Delete(Event @event);
}