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
        BirthDate = user.BirthDate;
        Email = user.Email;
        Sex = user.Sex;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public char Sex { get; set; }

    public static UserResult Create(User user) => new(user);
}
