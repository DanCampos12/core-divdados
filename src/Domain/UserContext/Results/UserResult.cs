using Core.Divdados.Domain.UserContext.Entities;
using System;

namespace Core.Divdados.Domain.UserContext.Results;

public class UserResult
{
    public UserResult() { }

    private UserResult(User user, Preference preference)
    {
        Id = user.Id;
        Name = user.Name;
        BirthDate = user.BirthDate;
        Email = user.Email;
        Sex = user.Sex;
        FlowComplete = user.FlowComplete;
        Preference = PreferenceResult.Create(preference);
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public char Sex { get; set; }
    public bool FlowComplete { get; set; }
    public PreferenceResult Preference { get; set; }

    public static UserResult Create(User user, Preference preference) => new(user, preference);
}