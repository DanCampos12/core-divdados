using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class RefreshTokenCommand : Command<RefreshTokenCommandResult>
{
    public string IdToken { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(IdToken, nameof(IdToken), "Token de acesso do usuário é obrigatório"));

        return Valid;
    }
}