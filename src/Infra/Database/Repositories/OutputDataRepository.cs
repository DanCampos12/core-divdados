using Core.Divdados.Domain.UserContext.Constants;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using Microsoft.EntityFrameworkCore;
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
    private IEnumerable<Category> _categories;

    public OutputDataRepository(UserDataContext context)
        => _context = context;

    public OutputDataResult GetOutputData(Guid userId, DateTime date)
    {
        _userId = userId;
        _date = date;
        _operations = GetOperations(userId, date);
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

    private AccumulatedValuesResult GetAccumulatedValues()
    {
        var currentDate = _operations.Min(x => x.Date);
        var valueByDate = 0.0M;
        List<ValueByDate> valuesByDates = new() { new(currentDate.AddDays(-1), valueByDate) };
        while (currentDate <= _date)
        {
            var inflowValue = _operations.Where(x => x.Type.Equals('I') && x.Date.Equals(currentDate)).Sum(x => x.Value);
            var outflowValue = _operations.Where(x => x.Type.Equals('O') && x.Date.Equals(currentDate)).Sum(x => x.Value);
            valueByDate += (inflowValue - outflowValue);
            valuesByDates.Add(new(currentDate, valueByDate));
            currentDate = currentDate.AddDays(1);
        }

        return new(
            description: "Patrimônio", 
            valuesByDates: valuesByDates);
    }

    private IEnumerable<CategoryAllocationResult> GetCategoryAllocations()
    {
        List<CategoryAllocationResult> categoryAllocations = new();
        
        if (!_operations.Any())
            return categoryAllocations;

        foreach (var category in _categories)
        {
            var categoryCount = (decimal)_operations.Where(x => x.CategoryId.Equals(category.Id)).Count();
            if (categoryCount > 0) 
                categoryAllocations.Add(new(category.Name, category.Color, (categoryCount / (decimal)_operations.Count())));
        }

        return categoryAllocations;
    }

    private IEnumerable<OperationTypeAllocationResult> GetOperationTypeAllocations()
    {
        List<OperationTypeAllocationResult> operationTypeAllocations = new();

        if (!_operations.Any())
            return operationTypeAllocations;

        var inflowCount = (decimal)_operations.Where(x => x.Type.Equals('I')).Count();
        var outflowCount = (decimal)_operations.Where(x => x.Type.Equals('O')).Count();
        operationTypeAllocations.Add(new(description: "Entradas", allocation: (inflowCount / (decimal)_operations.Count())));
        operationTypeAllocations.Add(new(description: "Saídas", allocation: (outflowCount / (decimal)_operations.Count())));

        return operationTypeAllocations;
    }

    private ObjectiveResult GetNextObjective()
    {
        var summary = GetSummary();
        var objecive = _context.Objectives
            .OrderBy(x => x.Order)
            .FirstOrDefault(x => x.Status.Equals(ObjectiveStatus.IN_PROGRESS));
        
        var progress = 0.0M;
        if (summary.TotalValue > 0.0M)
            progress = summary.TotalValue / objecive.Value;

        if (progress > 1.0M)
            progress = 1.0M;

        return ObjectiveResult.Create(objecive, progress);
    }

    private IEnumerable<OperationResult> GetNextOperations() => (
        from operation in _context.Operations
        join category in _context.Categories on operation.CategoryId equals category.Id
        where operation.UserId.Equals(_userId) && !operation.Effected && operation.Date >= _date
        orderby operation.Date, operation.Description
        select OperationResult.Create(operation, category))
        .Take(10);
}