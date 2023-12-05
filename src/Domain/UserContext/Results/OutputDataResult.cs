using Core.Divdados.Domain.UserContext.Entities;
using System;
using System.Collections.Generic;

namespace Core.Divdados.Domain.UserContext.Results;

public record SummaryResult (decimal TotalValue, decimal InflowValue, decimal OutflowValue);
public record ValueByDate (DateTime Date, decimal Value);
public record AccumulatedValuesResult (string Description, IEnumerable<ValueByDate> ValuesByDates);
public record CategoryAllocationResult(Guid Id, string Name, string Color, decimal Value, decimal Allocation, int Count);
public record OperationTypeAllocationResult(string Description, decimal Allocation, int Count);

public class OutputDataResult
{
    public OutputDataResult() { }

    private OutputDataResult(
        DateTime date, 
        SummaryResult summary,
        IEnumerable<AccumulatedValuesResult> accumulatedValues,
        IEnumerable<CategoryAllocationResult> categoryAllocations,
        IEnumerable<OperationTypeAllocationResult> operationTypeAllocations,
        ObjectiveResult nextObjective,
        IEnumerable<OperationResult> nextOperations)
    {
        Date = date;
        Summary = summary;
        AccumulatedValues = accumulatedValues;
        CategoryAllocations = categoryAllocations;
        OperationTypeAllocations = operationTypeAllocations;
        NextObjective = nextObjective;
        NextOperations = nextOperations;
    }

    public DateTime Date { get; set; }
    public SummaryResult Summary { get; set; }
    public IEnumerable<AccumulatedValuesResult> AccumulatedValues { get; set; }
    public IEnumerable<CategoryAllocationResult> CategoryAllocations { get; set; }
    public IEnumerable<OperationTypeAllocationResult> OperationTypeAllocations { get; set; }
    public ObjectiveResult NextObjective { get; set; }
    public IEnumerable<OperationResult> NextOperations { get; set; }

    public static OutputDataResult Create(
        DateTime date, 
        SummaryResult summary,
        IEnumerable<AccumulatedValuesResult> accumulatedValues,
        IEnumerable<CategoryAllocationResult> categoryAllocations,
        IEnumerable<OperationTypeAllocationResult> operationTypeAllocations,
        ObjectiveResult nextObjective,
        IEnumerable<OperationResult> nextOperations) => new(date, summary, accumulatedValues, categoryAllocations, operationTypeAllocations, nextObjective, nextOperations);
}
