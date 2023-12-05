using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Results;
using System;
using System.Collections.Generic;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface IObjectiveRepository
{
    Objective GetObjective(Guid id, Guid userId);
    IEnumerable<ObjectiveResult> GetObjectives(Guid userId);
    IEnumerable<ObjectiveResult> GetObjectives(Guid userId, DateTime date);
    ObjectiveResult Add(Objective objective);
    ObjectiveResult Update(Objective objective);
    ObjectiveResult Complete(Objective objective, bool shouldLaunchOperation);
    Guid Delete(Objective objective);
    IEnumerable<ObjectiveResult> Process(Guid userId);
    IEnumerable<ObjectiveResult> Reorder(Guid userId, IEnumerable<ObjectiveOrder> objectiveOrders);
}