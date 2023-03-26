using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Results;
using System;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface IUserRepository
{
    User Get(Guid id);
    User GetByEmail(string email);
    UserResult GetUserResult(Guid id);
    UserResult Add(User user);
    UserResult Update(User user);
    Guid Delete(User user);
}