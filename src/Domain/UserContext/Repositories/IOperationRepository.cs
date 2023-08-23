using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Results;
using System;
using System.Collections.Generic;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface IOperationRepository
{
    Operation GetOperation(Guid id, Guid userId);
    IEnumerable<OperationResult> GetOperations(Guid userId);
    OperationResult Add(Operation operation);
    OperationResult Update(Operation operation);
    Guid Delete(Operation operation);
}