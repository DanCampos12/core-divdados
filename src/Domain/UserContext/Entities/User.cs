using Core.Divdados.Shared.Entities;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Entities;

public sealed class User : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string Password { get; private set; }
    public char Sex { get; private set; }

    private User () { }
    public User(
        string name,
        string email,
        DateTime birthDate,
        string password,
        char sex) 
    {
        Name = name;
        Email = email;
        BirthDate = birthDate;
        Password = password;
        Sex = sex;

        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Name, nameof(Name), "Nome do usuário é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Name, 50, nameof(Name), "Nome do usuário não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(Email, nameof(Email), "Email do usuário é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Email, 100, nameof(Email), "Email do usuário não pode ter mais que 100 caracteres")
            .IsNotNullOrEmpty(Password, nameof(Password), "Senha do usuário é obrigatória"));
    }

    public void Update(string name, DateTime birthDate, char sex)
    {
        Name = name;
        BirthDate = birthDate;
        Sex = sex;
        AddNotifications(this);
    }

    public void UpdatePassword(string password)
    {
        Password = password;
    }
}