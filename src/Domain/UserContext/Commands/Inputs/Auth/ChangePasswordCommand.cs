using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class ChangePasswordCommand : Command<ChangePasswordCommandResult>
{
    public Guid UserId { get; set; }
    public string Password { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório")
            .IsNotNullOrEmpty(Password, nameof(Password), "Senha do usuário é obrigatória")
            .IsNotNullOrEmpty(NewPassword, nameof(NewPassword), "Nova senha do usuário é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(NewPassword, 100, nameof(NewPassword), "Senha não pode ter mais que 100 caracteres")
            .IsNotNullOrEmpty(ConfirmNewPassword, nameof(ConfirmNewPassword), "Confirmação de senha é obrigatória")
            .IsTrue(NewPassword.Equals(ConfirmNewPassword), nameof(NewPassword), "Senhas não correspondem"));

        return Valid;
    }
}
