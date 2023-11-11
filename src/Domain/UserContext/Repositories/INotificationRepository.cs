using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Results;
using System;
using System.Collections.Generic;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface INotificationRepository
{
    Notification Get(Guid Id);
    IEnumerable<NotificationResult> UpdateRange(IEnumerable<Notification> notifications);
    IEnumerable<NotificationResult> Process(Guid userId);
}