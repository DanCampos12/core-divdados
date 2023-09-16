using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using System;
using System.Linq;

namespace Core.Divdados.Infra.SQL.Repositories;

public class UserRepository : IUserRepository
{
    public UserDataContext _context;

    public UserRepository(UserDataContext context) => _context = context;

    public User Get(Guid id) => _context.Users
        .Where(user => user.Id.Equals(id))
        .FirstOrDefault();

    public User GetByEmail(string email) => _context.Users
        .Where(user => user.Email.Equals(email))
        .FirstOrDefault();

    public UserResult GetUserResult(Guid id) =>
        GetUserResultQuery(id).FirstOrDefault();


    public UserResult Add(User user) {
        _context.Users.Add(user);
        return UserResult.Create(user);
    }

    public UserResult Update(User user) {
        _context.Users.Update(user);
        return UserResult.Create(user);
    }

    public Guid Delete(User user)
    {
        _context.Users.Remove(user);
        return user.Id;
    }

    private IQueryable<UserResult> GetUserResultQuery(Guid id) =>
        from user in _context.Users
        where user.Id.Equals(id)
        select new UserResult
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            BirthDate = user.BirthDate,
            Sex = user.Sex
        };
}