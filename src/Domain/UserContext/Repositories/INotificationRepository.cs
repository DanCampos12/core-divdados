using Core.Divdados.Domain.UserContext.Results;
using System;
using System.Collections.Generic;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface INotificationRepository
{
    IEnumerable<NotificationResult> Process(Guid userId);
}