﻿using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class CreateUserCommandResult : CommandResult
{
    public UserResult User { get; set; }
}