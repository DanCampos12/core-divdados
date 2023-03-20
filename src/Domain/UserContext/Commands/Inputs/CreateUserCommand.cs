using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class CreateUserCommand : Command<CreateUserCommandResult>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public int Age { get; set; }
    public char Sex { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Name, nameof(Name), "Nome do usuário é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Name, 50, nameof(Name), "Nome do usuário não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(Surname, nameof(Surname), "Sobrenome do usuário é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Surname, 50, nameof(Surname), "Sobrenome do usuário não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(Email, nameof(Email), "Email do usuário é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Email, 50, nameof(Email), "Email do usuário não pode ter mais que 100 caracteres")
            .IsNotNullOrEmpty(Password, nameof(Password), "Senha do usuário é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Password, 50, nameof(Password), "Senha do usuário não pode ter mais que 100 caracteres")
            .IsGreaterThan(Age, 0, nameof(Age), "Idade do usuário é obrigatória e deve ser maior que 0 (zero)")
            .IsNotNullOrEmpty(Sex.ToString(), nameof(Age), "Sexo do usuário é obrigatório"));

        return Valid;
    }
}