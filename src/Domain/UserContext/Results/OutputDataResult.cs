using Core.Divdados.Domain.UserContext.Entities;
using System;
using System.Collections.Generic;

namespace Core.Divdados.Domain.UserContext.Results;

public record SummaryResult (decimal TotalValue, decimal InflowValue, decimal OutflowValue);
public record ValueByDate (DateTime date, decimal value);
public record AccumulatedValuesResult (string description, IEnumerable<ValueByDate> valuesByDates);
public record CategoryAllocationResult(string name, string color, decimal allocation);
public record OperationTypeAllocationResult(string description, decimal allocation);

public class OutputDataResult
{
    public OutputDataResult() { }

    private OutputDataResult(
        DateTime date, 
        SummaryResult summary, 
        AccumulatedValuesResult accumulatedValues,
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
    public AccumulatedValuesResult AccumulatedValues { get; set; }
    public IEnumerable<CategoryAllocationResult> CategoryAllocations { get; set; }
    public IEnumerable<OperationTypeAllocationResult> OperationTypeAllocations { get; set; }
    public ObjectiveResult NextObjective { get; set; }
    public IEnumerable<OperationResult> NextOperations { get; set; }

    public static OutputDataResult Create(
        DateTime date, 
        SummaryResult summary, 
        AccumulatedValuesResult accumulatedValues,
        IEnumerable<CategoryAllocationResult> categoryAllocations,
        IEnumerable<OperationTypeAllocationResult> operationTypeAllocations,
        ObjectiveResult nextObjective,
        IEnumerable<OperationResult> nextOperations) => new(date, summary, accumulatedValues, categoryAllocations, operationTypeAllocations, nextObjective, nextOperations);
}
