using Core.Divdados.Domain.UserContext.Entities;
using System;

namespace Core.Divdados.Domain.UserContext.Results;

public class UserResult
{
    public UserResult() { }

    private UserResult(User user)
    {
        Id = user.Id;
        Name = user.Name;
        Surname = user.Surname;
        Email = user.Email;
        Age = user.Age;
        Sex = user.Sex;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public char Sex { get; set; }

    public static UserResult Create(User user) => new(user);
}
