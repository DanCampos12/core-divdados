using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Results;
using System;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface IUserRepository
{
    User Get(Guid id);
    User GetByEmail(string email);
    Preference GetPreference(Guid userId);
    UserResult Add(User user);
    UserResult Update(User user);
    public UserResult UpdatePreference(User user, Preference preference);
}