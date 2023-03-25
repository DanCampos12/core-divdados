using Flunt.Validations;
using System;
using Core.Divdados.Shared.Entities;

namespace Core.Divdados.Domain.UserContext.Entities;

public sealed class User : Entity
{
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public int Age { get; private set; }
    public char Sex { get; private set; }
    public string LastSessionTokenId { get; private set; }

    private User () { }
    public User(
        string name,
        string surname,
        string email,
        string password,
        int age,
        char sex) 
    {
        Name = name;
        Surname = surname;
        Email = email;
        Password = password;
        Age = age;
        Sex = sex;

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
    }

    public void Update(string name, string surname, int age, char sex)
    {
        Name = name;
        Surname = surname;
        Age = age;
        Sex = sex;
        AddNotifications(this);
    } 
}