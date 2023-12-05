using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Results;
using System;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface IUserRepository
{
    User Get(Guid id);
    User GetByEmail(string email);
    Preference GetPreference(Guid userId);
    UserResult Add(User user);
    UserResult Update(User user);
    UserResult UpdatePreference(User user, Preference preference);
    Task<string> RecoverPassword(User user, string accessURL, string apiKey);
}