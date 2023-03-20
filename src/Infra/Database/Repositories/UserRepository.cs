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

    public UserRepository(UserDataContext context) =>
        _context = context;

    public void Add(User user) =>
        _context.Add(user);

    public void Update(User user) =>
        _context.Update(user);

    public void Delete(User user) =>
        _context.Remove(user);

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

    public UserResult Get(Guid id) =>
        GetUserResultQuery(id).FirstOrDefault();

    public bool CheckExist(string email) => _context.Users
        .Where(user => user.Email == email).Any();
}