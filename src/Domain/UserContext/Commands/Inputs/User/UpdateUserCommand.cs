using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class UpdateUserCommand : Command<UpdateUserCommandResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public char Sex { get; set; }
    public string Password { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Id.ToString(), nameof(Id), "Id do usuário é obrigatório")
            .IsNotNullOrEmpty(Name, nameof(Name), "Nome do usuário é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Name, 50, nameof(Name), "Nome do usuário não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(BirthDate.ToString(), nameof(BirthDate), "Date de nascimento do usuário é obrigatória")
            .IsNotNullOrEmpty(Sex.ToString(), nameof(Sex), "Sexo do usuário é obrigatório")
            .IsNotNullOrEmpty(Password, nameof(Password), "Senha do usuário é obrigatória"));

        return Valid;
    }
}