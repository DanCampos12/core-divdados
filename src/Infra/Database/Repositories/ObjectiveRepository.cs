using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Constants;
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
            if (objective.Status.Equals(ObjectiveStatus.IN_PROGRESS))
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

    public IEnumerable<ObjectiveResult> GetObjectives(Guid userId, DateTime date)
    {
        var objectivesResult = new List<ObjectiveResult>();
        var objectives = GetObjectivesQuery(userId);
        var totalValue = GetUserOperationsTotalValue(userId, date);
        if (totalValue < 0) totalValue = 0;

        foreach (var objective in objectives)
        {
            if (objective.Status.Equals(ObjectiveStatus.IN_PROGRESS))
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
        return ObjectiveResult.Create(objective, objective.Status.Equals("completed") ? 1.0M : 0.0M);
    }

    public ObjectiveResult Complete(Objective objective, bool shouldLaunchOperation)
    {
        _context.Objectives.Update(objective);

        if (shouldLaunchOperation)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Name.Equals("Objetivo") && x.IsAutomaticInput);
            _context.Operations.Add(new Operation(
                value: objective.Value,
                type: 'O',
                description: $"Conclusão de objetivo - {objective.Description}",
                date: objective.FinalDate,
                effected: true,
                userId: objective.UserId,
                categoryId: category.Id,
                eventId: null));
        }

        var objectivesInProgress = _context.Objectives
            .Where(x => x.UserId.Equals(objective.UserId) &&
                        x.Status.Equals("inProgress") &&
                        !x.Id.Equals(objective.Id))
            .Select(x => new ObjectiveOrder(x.Id, x.Order));
        Reorder(objective.UserId, objectivesInProgress);

        return ObjectiveResult.Create(objective, objective.Status.Equals("completed") ? 1.0M : 0.0M);
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
            .Where(x => x.UserId.Equals(userId) && 
                        x.FinalDate < DateTime.Today &&
                        x.Status.Equals(ObjectiveStatus.IN_PROGRESS))
            .ToArray();

        foreach (var objectiveToUpdateStatus in objectivesToUpdateStatus)
            objectiveToUpdateStatus.UpdateStatus(ObjectiveStatus.EXPIRED);

        var notifications = objectivesToUpdateStatus.Select(x => new Notification(
            $"O objetivo {x.Description} com data término em {x.FinalDate:dd/MM/yyyy} expirou!",
            NotificationTypes.OBJECTIVE_EXPIRED,
            false,
            x.UserId));
        
        _context.Objectives.UpdateRange(objectivesToUpdateStatus);
        _context.Notifications.AddRange(notifications);
        Console.WriteLine(notifications);
        return objectivesToUpdateStatus.Select(x => ObjectiveResult.Create(x, 0.0M));
    }

    public IEnumerable<ObjectiveResult> Reorder(Guid userId, IEnumerable<ObjectiveOrder> objectivesOrder)
    {
        var objectives = new List<Objective>();
        var order = 0;
        foreach (var objectiveOrder in objectivesOrder.OrderBy(x => x.Order))
        {
            var objective = GetObjective(objectiveOrder.Id, userId);
            objective.UpdateOrder(order);
            objectives.Add(objective);
            order++;
        }

        var objectivesNotUpdated = GetObjectivesNotInRangeQuery(userId, objectivesOrder.Select(x => x.Id));
        foreach (var objective in objectivesNotUpdated)
        {
            objective.UpdateOrder(order);
            objectives.Add(objective);
            order++;
        }

        _context.Objectives.UpdateRange(objectives);
        return objectives.Select(x => ObjectiveResult.Create(x, x.Status.Equals("completed") ? 1.0M : 0.0M));
    }

    private IQueryable<Objective> GetObjectivesQuery(Guid userId) =>
        from objective in _context.Objectives
        where objective.UserId.Equals(userId)
        orderby objective.Order
        select objective;

    private IQueryable<Objective> GetObjectivesNotInRangeQuery(Guid userId, IEnumerable<Guid> objectiveIds) =>
        from objective in _context.Objectives
        where objective.UserId.Equals(userId) && 
              !objectiveIds.Contains(objective.Id)
        orderby objective.Order
        select objective;

    private decimal GetUserOperationsTotalValue(Guid userId) => _context.Operations
        .Where(operation => operation.UserId.Equals(userId) && operation.Effected)
        .Sum(operation => operation.Type.ToString().Equals("I") ? operation.Value : (operation.Value * -1));

    private decimal GetUserOperationsTotalValue(Guid userId, DateTime date) => _context.Operations
        .Where(operation => operation.UserId.Equals(userId) && operation.Effected && operation.Date <= date)
        .Sum(operation => operation.Type.ToString().Equals("I") ? operation.Value : (operation.Value * -1));
}