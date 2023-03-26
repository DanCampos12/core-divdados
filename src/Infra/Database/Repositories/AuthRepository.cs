using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Infra.SQL.DataContext;
using System;
using System.Linq;

namespace Core.Divdados.Infra.SQL.Repositories;

public class AuthRepository : IAuthRepository
{
    public UserDataContext _context;

    public AuthRepository(UserDataContext context) => _context = context;

    public Guid UpdatePassword(User user, string password)
    {
        user.UpdatePassword(password);
        _context.Users.Update(user);
        return user.Id;
    }

    public Guid UpdateToken(User user, string idToken)
    {
        user.UpdateToken(idToken);
        _context.Users.Update(user);
        return user.Id;
    }
}