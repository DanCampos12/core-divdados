using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class CreateUserCommand : Command<CreateUserCommandResult>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public DateTime BirthDate { get; set; }
    public char Sex { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Name, nameof(Name), "Nome do usuário é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Name, 50, nameof(Name), "Nome do usuário não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(Email, nameof(Email), "Email do usuário é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Email, 50, nameof(Email), "Email do usuário não pode ter mais que 100 caracteres")
            .IsNotNullOrEmpty(Password, nameof(Password), "Senha do usuário é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Password, 100, nameof(Password), "Senha do usuário não pode ter mais que 100 caracteres")
            .IsNotNullOrEmpty(ConfirmPassword, nameof(Password), "Confirmação de senha é obrigatória")
            .IsTrue(Password.Equals(ConfirmPassword), nameof(Password), "Senhas não correspondem")
            .IsNotNullOrEmpty(BirthDate.ToString(), nameof(BirthDate), "Date de nascimento do usuário é obrigatória")
            .IsNotNullOrEmpty(Sex.ToString(), nameof(Sex), "Sexo do usuário é obrigatório"));

        return Valid;
    }
}