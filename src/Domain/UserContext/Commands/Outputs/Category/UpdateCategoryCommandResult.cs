using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class UpdateCategoryCommandResult : CommandResult
{
    public CategoryResult Category { get; set; }
}