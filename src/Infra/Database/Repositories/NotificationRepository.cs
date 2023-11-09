using Core.Divdados.Domain.UserContext.Constants;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using Core.Divdados.Shared.Uow;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;

namespace Core.Divdados.Infra.SQL.Repositories;

public class NotificationRepository : INotificationRepository
{
    public UserDataContext _context;
    private IUow _uow;
    private IObjectiveRepository _objectiveRepository;
    private IOperationRepository _operationRepository;
    private ICategoryRepository _categoryRepository;

    public NotificationRepository(
        UserDataContext context, 
        IUow uow,
        IObjectiveRepository objectiveRepository,
        IOperationRepository operationRepository,
        ICategoryRepository categoryRepository)
    {
        _context = context;
        _uow = uow;
        _objectiveRepository = objectiveRepository;
        _operationRepository = operationRepository;
        _categoryRepository = categoryRepository;
    }

    public IEnumerable<NotificationResult> Process(Guid userId)
    {
        // Remove notifications that are not in the date range
        var finalDate = DateTime.Today;
        var initialDate = finalDate.AddDays(-30);
        _context.Notifications.RemoveRange(_context.Notifications
            .Where(x => x.UserId.Equals(userId) && x.Date < initialDate));
        _uow.Commit();

        var notificationsToAdd = new List<Notification>();
        var notifications = _context.Notifications.Where(x => x.UserId.Equals(userId)).ToList();

        // Objective Notifications
        var objectivesInProgress = _objectiveRepository
            .GetObjectives(userId, finalDate)
            .Where(x => x.Status.Equals(ObjectiveStatus.IN_PROGRESS));

        foreach (var objective in objectivesInProgress)
        {
            var hasHalfCompletedNotification = notifications.Any(x =>
                x.ExternalId.Equals(objective.Id) &&
                x.Type.Equals(NotificationTypes.OBJECTIVE_HALF_COMPLETED));
            if (objective.Progress >= 0.5M && objective.Progress < 1.0M && !hasHalfCompletedNotification)
            {
                notificationsToAdd.Add(new Notification(
                    $"Parabéns! Você já completou 50% do objetivo {objective.Description}. " +
                    $"Continue assim, você está no caminho certo!",
                    NotificationTypes.OBJECTIVE_HALF_COMPLETED,
                    false,
                    userId,
                    objective.Id));
            }

            var hasFinishedNotification = notifications.Any(x =>
                x.ExternalId.Equals(objective.Id) &&
                x.Type.Equals(NotificationTypes.OBJECTIVE_FINISHED));
            if (objective.Progress == 1.0M && !hasFinishedNotification)
            {
                notificationsToAdd.Add(new Notification(
                    $"Parabéns! Você completou 100% do objetivo {objective.Description}. " +
                    $"Agora você está apto a concluí-lo!",
                    NotificationTypes.OBJECTIVE_FINISHED,
                    false,
                    userId,
                    objective.Id));
            }

            var hasExpiringInFiveDaysNotification = notifications.Any(x =>
                x.ExternalId.Equals(objective.Id) &&
                x.Type.Equals(NotificationTypes.OBJECTIVE_EXPIRING_IN_FIVE_DAYS));
            if (objective.FinalDate.AddDays(-5).Equals(finalDate) && !hasExpiringInFiveDaysNotification)
            {
                notificationsToAdd.Add(new Notification(
                    $"O objetivo {objective.Description} expira em 5 dias. " +
                    $"Mantenha o foco e continue trabalhando para atingir sua meta a tempo. " +
                    $"Você está no caminho certo! ",
                    NotificationTypes.OBJECTIVE_EXPIRING_IN_FIVE_DAYS,
                    false,
                    userId,
                    objective.Id));
            }

            var hasExpiringTomorrowNotification = notifications.Any(x =>
                x.ExternalId.Equals(objective.Id) &&
                x.Type.Equals(NotificationTypes.OBJECTIVE_EXPIRING_TOMORROW));
            if (objective.FinalDate.AddDays(-1).Equals(finalDate) && !hasExpiringTomorrowNotification)
            {
                notificationsToAdd.Add(new Notification(
                    $"O objetivo {objective.Description} está prestes a expirar! A data de conclusão é amanhã. " +
                    $"Não deixe passar essa oportunidade. Você está quase lá!",
                    NotificationTypes.OBJECTIVE_EXPIRING_TOMORROW,
                    false,
                    userId,
                    objective.Id));
            }
        }

        // Operations Notifications
        var operations = _operationRepository.GetOperations(userId, finalDate);
        var firstDayOfMonth = new DateTime(finalDate.Year, finalDate.Month, 1);
        var outflowValue = operations
            .Where(x => x.Date >= firstDayOfMonth && x.Type.Equals("O") && x.Effected)
            .Sum(x => x.Value);

        var hasFiveHundredOutflowNotification = notifications.Any(x =>
            x.Date >= firstDayOfMonth &&
            x.Type.Equals(NotificationTypes.OPERATION_FIVE_HUNDRED_OUTFLOW));
        if (outflowValue >= 500 && outflowValue < 1000 && !hasFiveHundredOutflowNotification)
        {
            notificationsToAdd.Add(new Notification(
                $"Seus gastos mensais ultrapassaram o valor de R$ 500,00!",
                NotificationTypes.OPERATION_FIVE_HUNDRED_OUTFLOW,
                false,
                userId,
                null));
        }

        var hasOneThousandOutflowNotification = notifications.Any(x =>
            x.Date >= firstDayOfMonth &&
            x.Type.Equals(NotificationTypes.OPERATION_ONE_THOUSAND_OUTFLOW));
        if (outflowValue >= 1000 && outflowValue < 2000 && !hasOneThousandOutflowNotification)
        {
            notificationsToAdd.Add(new Notification(
                $"Seus gastos mensais ultrapassaram o valor de R$ 1.000,00!",
                NotificationTypes.OPERATION_ONE_THOUSAND_OUTFLOW,
                false,
                userId,
                null));
        }

        var hasTwoThousandOutflowNotification = notifications.Any(x =>
            x.Date >= firstDayOfMonth &&
            x.Type.Equals(NotificationTypes.OPERATION_TWO_THOUSAND_OUTFLOW));
        if (outflowValue >= 2000 && outflowValue < 3000 && !hasTwoThousandOutflowNotification)
        {
            notificationsToAdd.Add(new Notification(
                $"Seus gastos mensais ultrapassaram o valor de R$ 2.000,00!",
                NotificationTypes.OPERATION_TWO_THOUSAND_OUTFLOW,
                false,
                userId,
                null));
        }

        var hasThreeThousandOutflowNotification = notifications.Any(x =>
            x.Date >= firstDayOfMonth &&
            x.Type.Equals(NotificationTypes.OPERATION_THREE_THOUSAND_OUTFLOW));
        if (outflowValue >= 3000 && outflowValue < 5000 && !hasThreeThousandOutflowNotification)
        {
            notificationsToAdd.Add(new Notification(
                $"Seus gastos mensais ultrapassaram o valor de R$ 3.000,00!",
                NotificationTypes.OPERATION_THREE_THOUSAND_OUTFLOW,
                false,
                userId,
                null));
        }

        var hasFiveThousandOutflowNotification = notifications.Any(x =>
            x.Date >= firstDayOfMonth &&
            x.Type.Equals(NotificationTypes.OPERATION_FIVE_THOUSAND_OUTFLOW));
        if (outflowValue >= 5000 && !hasFiveThousandOutflowNotification)
        {
            notificationsToAdd.Add(new Notification(
                $"Seus gastos mensais ultrapassaram o valor de R$ 5.000,00!",
                NotificationTypes.OPERATION_FIVE_THOUSAND_OUTFLOW,
                false,
                userId,
                null));
        }

        var operationsNotEffected = operations.Where(x => !x.Effected);
        foreach (var operation in operationsNotEffected)
        {
            var hasEffectOperationNotification = notifications.Any(x =>
                x.ExternalId.Equals(operation.Id) &&
                x.Type.Equals(NotificationTypes.EFFECT_OPERATION));

            if (!hasEffectOperationNotification)
            {
                notificationsToAdd.Add(new Notification(
                    $"A operação {operation.Description} já pode ser efetuada. " +
                    $"Verifique seus gastos/rendimentos para confirmar a movimentação!",
                    NotificationTypes.EFFECT_OPERATION,
                    false,
                    userId,
                    operation.Id));
            }
        }


        // Categories Notifications
        var categories = _categoryRepository.GetCategories(userId);
        List<CategoryAllocationResult> categoryAllocations = new();

        foreach (var category in categories)
        {
            var value = operations.Where(x => x.CategoryId.Equals(category.Id)).Sum(x => x.Type.Equals('I') ? x.Value : (x.Value * -1));
            categoryAllocations.Add(new(
                Id: category.Id,
                Name: category.Name,
                Color: category.Color,
                Value: value,
                Allocation: 0.0M,
                Count: 0));

            var hasLimitExceededNotification = notifications.Any(x =>
                x.ExternalId.Equals(category.Id) &&
                x.Type.Equals(NotificationTypes.CATEGORY_LIMIT_EXCEEDED));
            if (category.MaxValueMonthly is not null && Math.Abs(value) >= category.MaxValueMonthly && !hasLimitExceededNotification)
            {
                notificationsToAdd.Add(new Notification(
                    $"O limite mensal da categoria {category.Name} foi atingido. " +
                    $"Verifique suas movimentações para garantir que esteja alinhado com suas expectativas!",
                    NotificationTypes.CATEGORY_LIMIT_EXCEEDED,
                    false,
                    userId,
                    category.Id));
            }
        }

        if (finalDate.Equals(new DateTime(finalDate.AddMonths(1).Year, finalDate.AddMonths(1).Month, 1).AddDays(-1)))
        {
            var bestCategory = categoryAllocations.MaxBy(x => x.Value);
            var worstCategory = categoryAllocations.MinBy(x => x.Value);

            _context.Notifications.RemoveRange(notifications.Where(x =>
                x.Date >= firstDayOfMonth &&
                (!x.ExternalId.Equals(bestCategory.Id) && x.Type.Equals(NotificationTypes.CATEGORY_BEST_RESULTS) ||
                (!x.ExternalId.Equals(worstCategory.Id) && x.Type.Equals(NotificationTypes.CATEGORY_WORST_RESULTS)))));
            _uow.Commit();

            var hasBestResultsNotification = notifications.Any(x =>
                x.Date >= firstDayOfMonth &&
                x.ExternalId.Equals(bestCategory.Id) &&
                x.Type.Equals(NotificationTypes.CATEGORY_BEST_RESULTS));
            if (!hasBestResultsNotification && bestCategory.Value >= 0)
            {
                notificationsToAdd.Add(new Notification(
                    $"Último dia do mês! A categoria {bestCategory.Name} foi a que obteve mais lucro, " +
                    $"com um valor total de {bestCategory.Value}!",
                    NotificationTypes.CATEGORY_BEST_RESULTS,
                    false,
                    userId,
                    bestCategory.Id));
            }

            var hasWorstResultsNotification = notifications.Any(x =>
                x.Date >= firstDayOfMonth &&
                x.ExternalId.Equals(bestCategory.Id) &&
                x.Type.Equals(NotificationTypes.CATEGORY_BEST_RESULTS));
            if (!hasWorstResultsNotification && bestCategory.Value < 0)
            {
                notificationsToAdd.Add(new Notification(
                    $"Último dia do mês! A categoria {bestCategory.Name} foi a que obteve mais perda, " +
                    $"com um valor total de {Math.Abs(bestCategory.Value)}!",
                    NotificationTypes.CATEGORY_BEST_RESULTS,
                    false,
                    userId,
                    bestCategory.Id));
            }
            _uow.Commit();
        }

        _context.Notifications.AddRange(notificationsToAdd);
        _uow.Commit();

        return _context.Notifications.Where(x => x.UserId.Equals(userId))
            .OrderByDescending(x => x.Date)
            .Select(NotificationResult.Create);
    }
}