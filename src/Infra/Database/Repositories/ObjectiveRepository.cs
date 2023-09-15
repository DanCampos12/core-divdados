using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;

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
        var totalValue = GetUserOperationsTotalValue(userId);
        if (totalValue < 0) totalValue = 0;

        foreach (var objective in objectives)
        {
            if (objective.Status.Equals("inProgress"))
            {
                var progress = (totalValue / objective.Value);
                if (progress > 1) progress = 1.0M;
                objectivesResult.Add(ObjectiveResult.Create(objective, progress));
                totalValue -= objective.Value;
                if (totalValue < 0) totalValue = 0;
            } 
            else
            {
                var progress = objective.Status.Equals("completed") ? 1.0M : 0.0M;
                objectivesResult.Add(ObjectiveResult.Create(objective, progress));
            }
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
        var objectivesToUpdateOrder = _context.Objectives
            .Where(x => x.UserId.Equals(objective.UserId) && x.Order > objective.Order)
            .OrderBy(x => x.Order);

        var index = 0;
        foreach (var objectiveToUpdateOrder in objectivesToUpdateOrder)
        {
            objectiveToUpdateOrder.UpdateOrder(objective.Order + index);
            index++;
        }

        _context.Objectives.Remove(objective);
        _context.Objectives.UpdateRange(objectivesToUpdateOrder);
        return objective.Id;
    }

    public IEnumerable<ObjectiveResult> Process(Guid userId)
    {
        var objectivesToUpdateStatus = _context.Objectives
            .Where(x => x.UserId.Equals(userId) && x.FinalDate < DateTime.Today);

        foreach (var objectiveToUpdateStatus in objectivesToUpdateStatus)
            objectiveToUpdateStatus.UpdateStatus("expired");

        _context.Objectives.UpdateRange(objectivesToUpdateStatus);
        _context.Notifications.AddRange(objectivesToUpdateStatus.Select(x => new Notification(
            $"O objetivo {x.Description} com data término em {x.FinalDate:dd/MM/yyyy} expirou!",
            false,
            x.UserId)));

        return objectivesToUpdateStatus.Select(x => ObjectiveResult.Create(x, 0.0M));
    }

    private IQueryable<Objective> GetObjectivesQuery(Guid userId) =>
        from objective in _context.Objectives
        where objective.UserId.Equals(userId)
        orderby objective.Order
        select objective;

    private decimal GetUserOperationsTotalValue(Guid userId) => _context.Operations
        .Where(operation => operation.UserId.Equals(userId) && operation.Effected)
        .Sum(operation => operation.Type.ToString().Equals("I") ? operation.Value : (operation.Value * -1));
}