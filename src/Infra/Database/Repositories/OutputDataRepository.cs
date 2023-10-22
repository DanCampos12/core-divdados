using Core.Divdados.Domain.UserContext.Constants;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Divdados.Infra.SQL.Repositories;

public class OutputDataRepository : IOutputDataRepository
{
    public UserDataContext _context;
    private Guid _userId;
    private DateTime _date;
    private IEnumerable<Operation> _operations;
    private IEnumerable<Objective> _objectives;
    private IEnumerable<Category> _categories;

    public OutputDataRepository(UserDataContext context)
        => _context = context;

    public OutputDataResult GetOutputData(Guid userId, DateTime date)
    {
        _userId = userId;
        _date = date;
        _operations = GetOperations(userId, date);
        _objectives = GetObjetives(userId);
        _categories = GetCategories(userId);

        return OutputDataResult.Create(
            date: date,
            summary: GetSummary(),
            accumulatedValues: GetAccumulatedValues(),
            categoryAllocations: GetCategoryAllocations(),
            operationTypeAllocations: GetOperationTypeAllocations(),
            nextObjective: GetNextObjective(),
            nextOperations: GetNextOperations());
    }

    private IEnumerable<Operation> GetOperations(Guid userId, DateTime date)
        => _context.Operations
        .Where(x => x.UserId.Equals(userId) && x.Date <= date && x.Effected)
        .ToArray();

    private IEnumerable<Objective> GetObjetives(Guid userId)
        => _context.Objectives
        .Where(x => x.UserId.Equals(userId))
        .OrderBy(x => x.Order)
        .ToArray();

    private IEnumerable<Category> GetCategories(Guid userId)
        => _context.Categories
        .Where(x => x.UserId.Equals(userId))
        .OrderBy(x => x.Name)
        .ToArray();

    private SummaryResult GetSummary()
    {
        var inflowValue = _operations.Where(x => x.Type.Equals('I')).Sum(x => x.Value);
        var outflowValue = _operations.Where(x => x.Type.Equals('O')).Sum(x => x.Value);
        var totalValue = inflowValue - outflowValue;
        return new(totalValue, inflowValue, outflowValue);
    }

    private IEnumerable<AccumulatedValuesResult> GetAccumulatedValues()
    {
        if (!_operations.Any())
            return Array.Empty<AccumulatedValuesResult>();  

        var currentDate = _operations.Min(x => x.Date);
        var valueByDate = 0.0M;
        List<AccumulatedValuesResult> accumulatedValues = new();
        List<ValueByDate> patrimonyValuesByDates = new() { new(currentDate.AddDays(-1), valueByDate) };

        while (currentDate <= _date)
        {
            var inflowValue = _operations.Where(x => x.Type.Equals('I') && x.Date.Equals(currentDate)).Sum(x => x.Value);
            var outflowValue = _operations.Where(x => x.Type.Equals('O') && x.Date.Equals(currentDate)).Sum(x => x.Value);
            valueByDate += (inflowValue - outflowValue);
            patrimonyValuesByDates.Add(new(currentDate, valueByDate));
            currentDate = currentDate.AddDays(1);
        }
        accumulatedValues.Add(new(
            Description: "Patrimônio", 
            ValuesByDates: patrimonyValuesByDates));

        var nextObjecive = _objectives.FirstOrDefault(x => x.Status.Equals(ObjectiveStatus.IN_PROGRESS));
        if (nextObjecive is not null && patrimonyValuesByDates.Count > 1)
        {
            valueByDate = 0.0M;
            decimal offsetValue = nextObjecive.Value / (patrimonyValuesByDates.Count - 1);
            List<ValueByDate> objectiveValuesByDates = new();
            foreach (var item in patrimonyValuesByDates)
            {
                objectiveValuesByDates.Add(new(item.Date, Math.Round(valueByDate, 2)));
                valueByDate += offsetValue;
            }

            accumulatedValues.Add(new(
                Description: $"Objetivo - {nextObjecive.Description}",
                ValuesByDates: objectiveValuesByDates));
        }

        return accumulatedValues;
    }

    private IEnumerable<CategoryAllocationResult> GetCategoryAllocations()
    {
        List<CategoryAllocationResult> categoryAllocations = new();
        
        if (!_operations.Any())
            return categoryAllocations;

        foreach (var category in _categories)
        {
            var categoryCount = _operations.Where(x => x.CategoryId.Equals(category.Id)).Count();
            var value = _operations.Where(x => x.CategoryId.Equals(category.Id)).Sum(x => x.Type.Equals('I') ? x.Value : (x.Value * -1));
            if (categoryCount > 0) 
                categoryAllocations.Add(new(
                    Name: category.Name, 
                    Color: category.Color, 
                    Value: value, 
                    Allocation: ((decimal)categoryCount / (decimal)_operations.Count()), 
                    Count: categoryCount));
        }

        return categoryAllocations;
    }

    private IEnumerable<OperationTypeAllocationResult> GetOperationTypeAllocations()
    {
        List<OperationTypeAllocationResult> operationTypeAllocations = new();

        if (!_operations.Any())
            return operationTypeAllocations;

        var inflowCount = _operations.Where(x => x.Type.Equals('I')).Count();
        var outflowCount = _operations.Where(x => x.Type.Equals('O')).Count();
        operationTypeAllocations.Add(new(Description: "Entradas", Allocation: ((decimal)inflowCount / (decimal)_operations.Count()), Count: inflowCount));
        operationTypeAllocations.Add(new(Description: "Saídas", Allocation: ((decimal)outflowCount / (decimal)_operations.Count()), Count: outflowCount));

        return operationTypeAllocations;
    }

    private ObjectiveResult GetNextObjective()
    {
        var summary = GetSummary();
        var objecive = _objectives.FirstOrDefault(x => x.Status.Equals(ObjectiveStatus.IN_PROGRESS));

        if (objecive is null)
            return null;

        var progress = 0.0M;
        if (summary.TotalValue > 0.0M) progress = (summary.TotalValue / objecive.Value);
        if (progress > 1.0M) progress = 1.0M;

        return ObjectiveResult.Create(objecive, progress);
    }

    private IEnumerable<OperationResult> GetNextOperations() => (
        from operation in _context.Operations
        join category in _context.Categories on operation.CategoryId equals category.Id
        where operation.UserId.Equals(_userId) && !operation.Effected && operation.Date >= _date
        orderby operation.Date, operation.Description
        select OperationResult.Create(operation, category))
        .Take(5);
}