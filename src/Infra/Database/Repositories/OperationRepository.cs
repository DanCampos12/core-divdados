using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Divdados.Infra.SQL.Repositories;

public class OperationRepository : IOperationRepository
{
    public UserDataContext _context;

    public OperationRepository(UserDataContext context) => _context = context;

    public Operation GetOperation(Guid id, Guid userId) => _context.Operations
        .Where(operation => operation.Id.Equals(id) && operation.UserId.Equals(userId))
        .FirstOrDefault();

    public IEnumerable<OperationResult> GetOperations(Guid userId) =>
        GetOperationsQuery(userId).ToArray();

    public OperationResult Add(Operation operation) {
        _context.Operations.Add(operation);
        return OperationResult.Create(operation);
    }

    public OperationResult Update(Operation operation) {
        _context.Operations.Update(operation);
        return OperationResult.Create(operation);
    }

    public Guid Delete(Operation operation)
    {
        _context.Operations.Remove(operation);
        return operation.Id;
    }

    private IQueryable<OperationResult> GetOperationsQuery(Guid userId) =>
        from operation in _context.Operations
        where operation.UserId.Equals(userId)
        orderby operation.Date
        select new OperationResult
        {
            Id = operation.Id,
            Value = operation.Value,
            Type = operation.Type,
            Description = operation.Description,
            Date = operation.Date,
            Effected = operation.Effected,
            UserId = operation.UserId,
            CategoryId = operation.CategoryId,
            EventId = operation.EventId
        };
}