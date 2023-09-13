using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;

namespace Core.Divdados.Infra.SQL.Repositories;

public class ObjectiveRepository : IObjectiveRepository
{
    public UserDataContext _context;

    public ObjectiveRepository(UserDataContext context) => _context = context;

    public Objective GetObjective(Guid id, Guid userId) => _context.Objectives
        .Where(objective => objective.Id.Equals(id) && objective.UserId.Equals(userId))
        .FirstOrDefault();

    public IEnumerable<ObjectiveResult> GetObjectives(Guid userId)
    {
        var objectivesResult = new List<ObjectiveResult>();
        var objectives = GetObjectivesQuery(userId);
        var totalValue = GetTotalValue(userId);
        if (totalValue < 0) totalValue = 0;

        foreach (var objective in objectives)
        {
            var progress = (totalValue / objective.Value);
            if (progress > 1) progress = 1.0M;
            objectivesResult.Add(ObjectiveResult.Create(objective, progress));
            totalValue -= objective.Value;
            if (totalValue < 0) totalValue = 0;
        }

        return objectivesResult;
    }

    public ObjectiveResult Add(Objective objective) {
        _context.Objectives.Add(objective);
        return ObjectiveResult.Create(objective, 0.0M);
    }

    public ObjectiveResult Update(Objective objective) {
        _context.Objectives.Update(objective);
        return ObjectiveResult.Create(objective, 0.0M);
    }

    public Guid Delete(Objective objective)
    {
        _context.Objectives.Remove(objective);
        return objective.Id;
    }

    private IQueryable<Objective> GetObjectivesQuery(Guid userId) =>
        from objective in _context.Objectives
        where objective.UserId.Equals(userId)
        orderby objective.Order
        select objective;

    private decimal GetTotalValue(Guid userId) => _context.Operations
        .Where(operation => operation.UserId.Equals(userId))
        .Sum(operation => operation.Type.ToString().Equals("I") ? operation.Value : (operation.Value * -1));
}