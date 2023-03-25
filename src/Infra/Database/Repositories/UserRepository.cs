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

    public UserResult GetResult(Guid id) =>
        GetUserResultQuery(id).FirstOrDefault();


    public UserResult Add(User user) {
        _context.Add(user);
        return UserResult.Create(user);
    }

    public UserResult Update(User user) {
        _context.Update(user);
        return UserResult.Create(user);
    }

    public Guid Delete(User user)
    {
        _context.Remove(user);
        return user.Id;
    }

    public bool CheckExist(string email) => _context.Users
        .Where(user => user.Email.Equals(email))
        .Any();

    private IQueryable<UserResult> GetUserResultQuery(Guid id) =>
        from user in _context.Users
        where user.Id.Equals(id)
        select new UserResult
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Age = user.Age,
            Sex = user.Sex
        };
}