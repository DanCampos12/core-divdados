using Core.Divdados.Shared.Commands;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class DeleteCategoryCommandResult : CommandResult
{
    public Guid Id { get; set; }
}
