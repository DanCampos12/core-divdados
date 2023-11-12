using Core.Divdados.Domain.UserContext.Constants;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using Core.Divdados.Shared.Uow;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Divdados.Infra.SQL.Repositories;

public class NotificationRepository : INotificationRepository
{
    public UserDataContext _context;
    private readonly IUow _uow;
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly IOperationRepository _operationRepository;
    private readonly ICategoryRepository _categoryRepository;

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

    public Notification Get(Guid Id) =>
        _context.Notifications.FirstOrDefault(x => x.Id.Equals(Id));

    public IEnumerable<NotificationResult> UpdateRange(IEnumerable<Notification> notifications)
    {
        _context.Notifications.UpdateRange(notifications);
        return notifications.Select(NotificationResult.Create);
    }

    public IEnumerable<NotificationResult> Process(Guid userId)
    {
        var today = DateTime.Today;
        var newNotifications = new List<Notification>();
        var objectives = _objectiveRepository.GetObjectives(userId, today)
            .Where(x => x.Status.Equals(ObjectiveStatus.IN_PROGRESS));
        var operations = _operationRepository.GetOperations(userId, today);
        var categories = _categoryRepository.GetCategories(userId);

        DeleteNotifications(userId, today);
        var currentNotifications = _context.Notifications.Where(x => x.UserId.Equals(userId)).ToList();
        newNotifications.AddRange(ProcessOperationsNotifications(userId, today, currentNotifications, operations));
        newNotifications.AddRange(ProcessCategoriesNotifications(userId, today, currentNotifications, categories, operations));
        newNotifications.AddRange(ProcessObjectivesNotifications(userId, today, currentNotifications, objectives));

        _context.Notifications.AddRange(newNotifications);
        _uow.Commit();

        return (from notification in _context.Notifications
                where notification.UserId.Equals(userId) && !notification.Removed
                orderby notification.Read ascending, notification.Date descending
                select NotificationResult.Create(notification))
            .ToArray();
    }

    private void DeleteNotifications(Guid userId, DateTime today)
    {
        _context.Notifications.RemoveRange(_context.Notifications.Where(x =>
            x.UserId.Equals(userId) &&
            x.Date < today.AddDays(-30)));
        _uow.Commit();
    }

    private List<Notification> ProcessObjectivesNotifications(
        Guid userId,
        DateTime today,
        List<Notification> currentNotifications,
        IEnumerable<ObjectiveResult> objectives)
    {
        var newNotifications = new List<Notification>();
        foreach (var objective in objectives)
        {
            var halfCompletedNotification = currentNotifications.FirstOrDefault(x =>
                x.ExternalId.Equals(objective.Id) &&
                x.Type.Equals(NotificationTypes.OBJECTIVE_HALF_COMPLETED));
            if (objective.Progress >= 0.5M && objective.Progress < 1.0M)
            {
                if (halfCompletedNotification is null)
                    newNotifications.Add(new Notification(
                        title: "Objetivo 50% atingido",
                        message: $"Parabéns! Você já completou 50% do objetivo {objective.Description.ToLower()}. " +
                        $"Continue assim, você está no caminho certo!",
                        type: NotificationTypes.OBJECTIVE_HALF_COMPLETED,
                        userId: userId,
                        externalId: objective.Id));
            } 
            else if (halfCompletedNotification is not null)
                _context.Notifications.Remove(halfCompletedNotification);

            var finishedNotification = currentNotifications.FirstOrDefault(x =>
                x.ExternalId.Equals(objective.Id) &&
                x.Type.Equals(NotificationTypes.OBJECTIVE_FINISHED));
            if (objective.Progress == 1.0M)
            {
                if (finishedNotification is null)
                    newNotifications.Add(new Notification(
                        title: "Objetivo 100% atingido",
                        message: $"Parabéns! Você completou 100% do objetivo {objective.Description.ToLower()}. " +
                        $"Agora você está apto a concluí-lo!",
                        type: NotificationTypes.OBJECTIVE_FINISHED,
                        userId: userId,
                        externalId: objective.Id));
            }
            else if (finishedNotification is not null)
                _context.Notifications.Remove(finishedNotification);

            var expiringInFiveDaysNotification = currentNotifications.FirstOrDefault(x =>
                x.ExternalId.Equals(objective.Id) &&
                x.Type.Equals(NotificationTypes.OBJECTIVE_EXPIRING_IN_FIVE_DAYS));
            if (objective.FinalDate.AddDays(-5).Date.Equals(today.Date))
            {
                if (expiringInFiveDaysNotification is null)
                    newNotifications.Add(new Notification(
                        title: "Objetivo à expirar",
                        message: $"O objetivo {objective.Description.ToLower()} expira em 5 dias. " +
                        $"Mantenha o foco e continue trabalhando para atingir sua meta a tempo. " +
                        $"Você está no caminho certo! ",
                        type: NotificationTypes.OBJECTIVE_EXPIRING_IN_FIVE_DAYS,
                        userId: userId,
                        externalId: objective.Id));
            }
            else if (expiringInFiveDaysNotification is not null)
                _context.Notifications.Remove(expiringInFiveDaysNotification);

            var expiringTomorrowNotification = currentNotifications.FirstOrDefault(x =>
                x.ExternalId.Equals(objective.Id) &&
                x.Type.Equals(NotificationTypes.OBJECTIVE_EXPIRING_TOMORROW));
            if (objective.FinalDate.AddDays(-1).Date.Equals(today.Date))
            {
                if (expiringTomorrowNotification is null)
                    newNotifications.Add(new Notification(
                        title: "Objetivo à expirar",
                        message: $"O objetivo {objective.Description.ToLower()} está prestes a expirar! A data de conclusão é amanhã. " +
                        $"Não deixe passar essa oportunidade. Você está quase lá!",
                        type: NotificationTypes.OBJECTIVE_EXPIRING_TOMORROW,
                        userId: userId,
                        externalId: objective.Id));
            }
            else if (expiringTomorrowNotification is not null)
                _context.Notifications.Remove(expiringInFiveDaysNotification);
        }

        _uow.Commit();
        return newNotifications;
    }

    private List<Notification> ProcessOperationsNotifications(
        Guid userId,
        DateTime today,
        List<Notification> currentNotifications,
        IEnumerable<OperationResult> operations)
    {
        var newNotifications = new List<Notification>();
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1).Date;
        var outflowValue = operations
            .Where(x => x.Date >= firstDayOfMonth && x.Type.Equals('O') && x.Effected)
            .Sum(x => x.Value);

        var fiveHundredOutflowNotification = currentNotifications.FirstOrDefault(x =>
            x.Date >= firstDayOfMonth &&
            x.Type.Equals(NotificationTypes.OPERATION_FIVE_HUNDRED_OUTFLOW));
        if (outflowValue >= 500 && outflowValue < 1000)
        {
            if (fiveHundredOutflowNotification is null)
                newNotifications.Add(new Notification(
                    title: "Alerta de gastos",
                    message: $"Seus gastos mensais ultrapassaram o limite de R$ 500,00. " +
                    $"Recomendamos revisar seu orçamento para garantir um gerenciamento financeiro saudável.",
                    type: NotificationTypes.OPERATION_FIVE_HUNDRED_OUTFLOW,
                    userId: userId,
                    externalId: null));
        }
        else if (fiveHundredOutflowNotification is not null)
            _context.Notifications.Remove(fiveHundredOutflowNotification);

        var oneThousandOutflowNotification = currentNotifications.FirstOrDefault(x =>
            x.Date >= firstDayOfMonth &&
            x.Type.Equals(NotificationTypes.OPERATION_ONE_THOUSAND_OUTFLOW));
        if (outflowValue >= 1000 && outflowValue < 2000)
        {
            if (oneThousandOutflowNotification is null)
                newNotifications.Add(new Notification(
                    title: "Alerta de gastos",
                    message: $"Seus gastos mensais ultrapassaram o limite de R$ 1.000,00. " +
                    $"Recomendamos uma análise cuidadosa do seu orçamento para manter o equilíbrio financeiro.",
                    type: NotificationTypes.OPERATION_ONE_THOUSAND_OUTFLOW,
                    userId: userId,
                    externalId: null));
        }
        else if (oneThousandOutflowNotification is not null)
            _context.Notifications.Remove(oneThousandOutflowNotification);

        var twoThousandOutflowNotification = currentNotifications.FirstOrDefault(x =>
            x.Date >= firstDayOfMonth &&
            x.Type.Equals(NotificationTypes.OPERATION_TWO_THOUSAND_OUTFLOW));
        if (outflowValue >= 2000 && outflowValue < 3000)
        {
            if (twoThousandOutflowNotification is null)
                newNotifications.Add(new Notification(
                    title: "Alerta de despesas",
                    message: $"Observamos que seus gastos mensais ultrapassaram o valor de R$ 2.000,00. " +
                    $"Recomendamos uma revisão do seu orçamento para garantir uma gestão financeira eficiente e equilibrada.",
                    type: NotificationTypes.OPERATION_TWO_THOUSAND_OUTFLOW,
                    userId: userId,
                    externalId: null));
        }
        else if (twoThousandOutflowNotification is not null)
            _context.Notifications.RemoveRange(twoThousandOutflowNotification);

        var threeThousandOutflowNotification = currentNotifications.FirstOrDefault(x =>
            x.Date >= firstDayOfMonth &&
            x.Type.Equals(NotificationTypes.OPERATION_THREE_THOUSAND_OUTFLOW));
        if (outflowValue >= 3000 && outflowValue < 5000)
        {
            if (threeThousandOutflowNotification is null) 
                newNotifications.Add(new Notification(
                    title: "Alerta de despesas",
                    message: $"Notamos que seus gastos mensais excederam o valor de R$ 3.000,00. " +
                    $"Recomendamos uma análise detalhada do seu orçamento para assegurar uma gestão financeira sustentável.",
                    type: NotificationTypes.OPERATION_THREE_THOUSAND_OUTFLOW,
                    userId: userId,
                    externalId: null));
        } else if (threeThousandOutflowNotification is not null)
            _context.Notifications.Remove(threeThousandOutflowNotification);

        var fiveThousandOutflowNotification = currentNotifications.FirstOrDefault(x =>
            x.Date >= firstDayOfMonth &&
            x.Type.Equals(NotificationTypes.OPERATION_FIVE_THOUSAND_OUTFLOW));
        if (outflowValue >= 5000)
        {
            if (fiveThousandOutflowNotification is null)
                newNotifications.Add(new Notification(
                    title: "Alerta de despesas",
                    message: $"Informamos que seus gastos mensais ultrapassaram o valor de R$ 5.000,00. " +
                    $"Sugerimos uma revisão cuidadosa do seu orçamento para garantir uma gestão financeira sólida.",
                    type: NotificationTypes.OPERATION_FIVE_THOUSAND_OUTFLOW,
                    userId: userId,
                    externalId: null));
        } else if (fiveHundredOutflowNotification is not null)
            _context.Notifications.Remove(fiveThousandOutflowNotification);

        var operationsNotEffected = operations.Where(x => !x.Effected);
        foreach (var operation in operationsNotEffected)
        {
            var hasEffectOperationNotification = currentNotifications.Any(x =>
                x.ExternalId.Equals(operation.Id) &&
                x.Type.Equals(NotificationTypes.EFFECT_OPERATION));
            if (!hasEffectOperationNotification)
            {
                newNotifications.Add(new Notification(
                    title: "Operação à efetuar",
                    message: $"A operação {operation.Description.ToLower()} - {operation.Date:dd/MM/yyyy} já pode ser efetuada. " +
                    $"Recomendamos que você verifique seus registros e/ou extratos para confirmar se " +
                    $"a operação foi concluída com sucesso antes de prosseguir com sua efetuação.",
                    NotificationTypes.EFFECT_OPERATION,
                    userId: userId,
                    externalId: operation.Id));
            }
        }

        _context.Notifications.RemoveRange(currentNotifications.Where(x =>
            x.Type.Equals(NotificationTypes.EFFECT_OPERATION) &&
            operations.Any(y => y.Id.Equals(x.ExternalId) && y.Effected)));

        _uow.Commit();
        return newNotifications;
    }

    private List<Notification> ProcessCategoriesNotifications(
        Guid userId,
        DateTime today,
        List<Notification> currentNotifications,
        IEnumerable<CategoryResult> categories,
        IEnumerable<OperationResult> operations)
    {
        var newNotifications = new List<Notification>();
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1).Date;
        var categoryAllocations = new List<CategoryAllocationResult>();
        foreach (var category in categories)
        {
            var value = operations.Where(x => 
                x.Date >= firstDayOfMonth && 
                x.CategoryId.Equals(category.Id))
                .Sum(x => x.Type.Equals('I') ? x.Value : (x.Value * -1));
            categoryAllocations.Add(new(
                Id: category.Id,
                Name: category.Name,
                Color: category.Color,
                Value: value,
                Allocation: 0.0M,
                Count: 0));

            var limitExceededNotification = currentNotifications.FirstOrDefault(x =>
                x.ExternalId.Equals(category.Id) &&
                x.Type.Equals(NotificationTypes.CATEGORY_LIMIT_EXCEEDED));
            if (category.MaxValueMonthly is not null && Math.Abs(value) >= category.MaxValueMonthly)
            {
                if (limitExceededNotification is null)
                    newNotifications.Add(new Notification(
                        title: "Limite de categoria atingido",
                        message: $"O limite mensal da categoria {category.Name.ToLower()} foi atingido. " +
                        $"Verifique suas movimentações para garantir que esteja alinhado com suas expectativas.",
                        type: NotificationTypes.CATEGORY_LIMIT_EXCEEDED,
                        userId: userId,
                        externalId: category.Id));
            }
            else if (limitExceededNotification is not null)
                _context.Notifications.Remove(limitExceededNotification);
        }

        if (today.Date.Equals(new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1).Date))
        {
            var bestCategory = categoryAllocations.MaxBy(x => x.Value);
            _context.Notifications.RemoveRange(currentNotifications.Where(x =>
                x.Date >= firstDayOfMonth && 
                x.Type.Equals(NotificationTypes.CATEGORY_BEST_RESULTS) &&
                !x.ExternalId.Equals(bestCategory.Id)));

            var bestResultsNotification = currentNotifications.FirstOrDefault(x =>
                x.Date >= firstDayOfMonth &&
                x.ExternalId.Equals(bestCategory.Id) &&
                x.Type.Equals(NotificationTypes.CATEGORY_BEST_RESULTS));
            if (bestCategory.Value >= 0)
            {
                if (bestResultsNotification is null)
                    newNotifications.Add(new Notification(
                        title: "Melhor categoria no mês",
                        message: $"Informamos que, neste mês, a categoria {bestCategory.Name.ToLower()} registrou o maior lucro entre as demais. " +
                        $"Agradecemos pelo seu comprometimento e eficiência na gestão financeira.",
                        NotificationTypes.CATEGORY_BEST_RESULTS,
                        userId: userId,
                        externalId: bestCategory.Id));
            }
            else if (bestResultsNotification is not null)
                _context.Notifications.Remove(bestResultsNotification);

            var worstCategory = categoryAllocations.MinBy(x => x.Value);
            _context.Notifications.RemoveRange(currentNotifications.Where(x =>
                x.Date >= firstDayOfMonth &&
                x.Type.Equals(NotificationTypes.CATEGORY_WORST_RESULTS) &&
                !x.ExternalId.Equals(worstCategory.Id)));

            var worstResultsNotification = currentNotifications.FirstOrDefault(x =>
                x.Date >= firstDayOfMonth &&
                x.ExternalId.Equals(worstCategory.Id) &&
                x.Type.Equals(NotificationTypes.CATEGORY_WORST_RESULTS));
            if (worstCategory.Value < 0)
            {
                if (worstResultsNotification is null)
                    newNotifications.Add(new Notification(
                        title: "Pior categoria no mês",
                        message: $"No decorrer deste mês, observamos que a categoria {worstCategory.Name.ToLower()} apresentou a maior perda. " +
                        $"Solicitamos sua atenção para avaliar e ajustar estratégias conforme necessário.",
                        NotificationTypes.CATEGORY_WORST_RESULTS,
                        userId: userId,
                        externalId: bestCategory.Id));
            }
            else if (worstResultsNotification is not null)
                _context.Notifications.Remove(worstResultsNotification);
        }

        _uow.Commit();
        return newNotifications;
    }
}