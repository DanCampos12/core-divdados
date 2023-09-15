using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class UpdateUserCommand : Command<UpdateUserCommandResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public int Age { get; set; }
    public char Sex { get; set; }
    public string Password { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Id.ToString(), nameof(Id), "Id do usuário é obrigatório")
            .IsNotNullOrEmpty(Name, nameof(Name), "Nome do usuário é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Name, 50, nameof(Name), "Nome do usuário não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(Surname, nameof(Surname), "Sobrenome do usuário é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Surname, 50, nameof(Surname), "Sobrenome do usuário não pode ter mais que 50 caracteres")
            .IsGreaterThan(Age, 0, nameof(Age), "Idade do usuário é obrigatória e deve ser maior que 0 (zero)")
            .IsNotNullOrEmpty(Sex.ToString(), nameof(Age), "Sexo do usuário é obrigatório")
            .IsNotNullOrEmpty(Password, nameof(Password), "Senha do usuário é obrigatória"));

        return Valid;
    }
}