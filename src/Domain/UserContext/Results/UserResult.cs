using System;

namespace Core.Divdados.Domain.UserContext.Results;

public class UserResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public char Sex { get; set; }
}
