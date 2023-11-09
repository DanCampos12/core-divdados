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

    public IEnumerable<OperationResult> GetOperations(Guid userId, DateTime date) =>
        GetOperationsQuery(userId, date).ToArray();

    public OperationResult Add(Operation operation) {
        _context.Operations.Add(operation);
        var category = _context.Categories.FirstOrDefault(x => x.Id.Equals(operation.CategoryId));
        return OperationResult.Create(operation, category);
    }

    public OperationResult Update(Operation operation) {
        _context.Operations.Update(operation);
        var category = _context.Categories.FirstOrDefault(x => x.Id.Equals(operation.CategoryId));
        return OperationResult.Create(operation, category);
    }

    public Guid Delete(Operation operation)
    {
        _context.Operations.Remove(operation);
        return operation.Id;
    }

    private IQueryable<OperationResult> GetOperationsQuery(Guid userId) =>
        from operation in _context.Operations
        join category in _context.Categories on operation.CategoryId equals category.Id
        where operation.UserId.Equals(userId)
        orderby operation.Date descending, operation.Description ascending
        select OperationResult.Create(operation, category);

    private IQueryable<OperationResult> GetOperationsQuery(Guid userId, DateTime date) =>
        from operation in _context.Operations
        join category in _context.Categories on operation.CategoryId equals category.Id
        where operation.UserId.Equals(userId) && operation.Date <= date
        orderby operation.Date descending, operation.Description ascending
        select OperationResult.Create(operation, category);
}