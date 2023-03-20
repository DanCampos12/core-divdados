using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Results;
using System;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface IUserRepository
{
    void Add(User user);
    void Update(User user);
    void Delete(User user);
    UserResult Get(Guid id);
    bool CheckExist(string email);
}