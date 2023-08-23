﻿using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class CreateOperationCommandResult : CommandResult
{
    public OperationResult Operation { get; set; }
}