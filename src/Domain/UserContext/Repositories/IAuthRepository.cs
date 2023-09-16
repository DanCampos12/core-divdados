using Core.Divdados.Domain.UserContext.Entities;
using System;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface IAuthRepository
{
    public Guid UpdatePassword(User user, string password);
}