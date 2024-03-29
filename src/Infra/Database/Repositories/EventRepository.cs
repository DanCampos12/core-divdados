﻿using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using Core.Divdados.Shared.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Divdados.Infra.SQL.Repositories;

public class EventRepository : IEventRepository
{
    public UserDataContext _context;
    private readonly IUow _uow;

    public EventRepository(UserDataContext context, IUow uow)
    {
        _context = context;
        _uow = uow;
    }

    public Event GetEvent(Guid id, Guid userId) => _context.Events
        .Where(x => x.Id.Equals(id) && x.UserId.Equals(userId))
        .FirstOrDefault();

    public IEnumerable<EventResult> GetEvents(Guid userId) =>
        GetEventsQuery(userId).ToArray();

    public IEnumerable<EventResult> GetEvents(Guid userId, DateTime date) =>
        GetEventsQuery(userId, date).ToArray();

    public EventResult Add(Event @event) {
        var category = _context.Categories.FirstOrDefault(x => x.Id.Equals(@event.CategoryId));
        var operationsDate = GetOperationDates(@event.InitialDate, @event.FinalDate, @event.Period);
        var operationsToInsert = new List<Operation>();

        foreach(var operationDate in operationsDate) 
        {
            operationsToInsert.Add(new Operation(
                value: @event.Value,
                type: @event.Type,
                description: @event.Description,
                date: operationDate,
                effected: operationDate <= DateTime.Today,
                userId: @event.UserId,
                categoryId: @event.CategoryId,
                eventId: @event.Id));
        }

        _context.Events.Add(@event);
        _context.Operations.AddRange(operationsToInsert);
        return EventResult.Create(@event, category, !operationsToInsert.Any(operation => !operation.Effected));
    }

    public EventResult Update(Event @event) {
        var category = _context.Categories.FirstOrDefault(x => x.Id.Equals(@event.CategoryId));
        var operationsToUpdate = _context.Operations.Where(x => x.EventId.Equals(@event.Id));

        foreach (var operation in operationsToUpdate)
            operation.Update(@event.Value, operation.Type, @event.Description, operation.Date, @event.CategoryId);

        _context.Events.Update(@event);
        _context.Operations.UpdateRange(operationsToUpdate);
        return EventResult.Create(@event, category, !operationsToUpdate.Any(operation => !operation.Effected));
    }

    public Guid Delete(Event @event)
    {
        var operationsToDelete = _context.Operations.Where(x => x.EventId.Equals(@event.Id));
        _context.Operations.RemoveRange(operationsToDelete);
        _uow.Commit();

        _context.Events.Remove(@event);
        return @event.Id;
    }

    private IQueryable<EventResult> GetEventsQuery(Guid userId) =>
        from @event in _context.Events
        join category in _context.Categories on @event.CategoryId equals category.Id
        where @event.UserId.Equals(userId)
        orderby @event.InitialDate descending, @event.Description ascending
        select EventResult.Create(@event, category, !_context.Operations
            .Where(x => x.EventId.Equals(@event.Id))
            .Any(x => !x.Effected));

    private IQueryable<EventResult> GetEventsQuery(Guid userId, DateTime date) =>
        from @event in _context.Events
        join category in _context.Categories on @event.CategoryId equals category.Id
        where @event.UserId.Equals(userId) && @event.InitialDate <= date
        orderby @event.InitialDate descending, @event.Description ascending
        select EventResult.Create(@event, category, !_context.Operations
            .Where(x => x.EventId.Equals(@event.Id))
            .Any(x => !x.Effected));

    private static IEnumerable<DateTime> GetOperationDates(DateTime initialDate, DateTime finalDate, string period)
    {
        var isEndOfMonth = initialDate.Day == DateTime.DaysInMonth(initialDate.Year, initialDate.Month);
        var operationDate = initialDate;
        var dates = new List<DateTime>();
        do
        {
            dates.Add(operationDate);
            operationDate = period switch
            {
                "7D" => operationDate.AddDays(7),
                "15D" => operationDate.AddDays(15),
                "1M" => AddMonths(operationDate, 1, isEndOfMonth),
                "2M" => AddMonths(operationDate, 2, isEndOfMonth),
                "3M" => AddMonths(operationDate, 3, isEndOfMonth),
                "6M" => AddMonths(operationDate, 6, isEndOfMonth),
                "9M" => AddMonths(operationDate, 9, isEndOfMonth),
                "1Y" => operationDate.AddYears(1),
                _ => finalDate.AddDays(1),
            };
        } while (operationDate <= finalDate);
        
        return dates;
    }

    private static DateTime AddMonths (DateTime baseDate, int months, bool isEndOfMonth)
    {
        var newDate = baseDate.AddMonths(months);
        if (isEndOfMonth) newDate = new DateTime(newDate.Year, newDate.Month, 1).AddMonths(1).AddDays(-1);
        return newDate;
    }
}