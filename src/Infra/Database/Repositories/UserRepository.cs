using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using Core.Divdados.Infra.SQL.Transactions;
using Core.Divdados.Shared.Uow;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Divdados.Infra.SQL.Repositories;

public record DefaultCategory(string name, string color);

public class UserRepository : IUserRepository
{
    public UserDataContext _context;
    private IUow _uow;

    public UserRepository(UserDataContext context, IUow uow)
    {
        _context = context;
        _uow = uow;
    }

    public User Get(Guid id) => _context.Users
        .Where(user => user.Id.Equals(id))
        .FirstOrDefault();

    public User GetByEmail(string email) => _context.Users
        .Where(user => user.Email.Equals(email))
        .FirstOrDefault();

    public UserResult Add(User user) {
        _context.Users.Add(user);
        _uow.Commit();

        var preference = new Preference(user.Id);
        List<DefaultCategory> defaultCategories = new()
        {
            new("Objetivo", "#2196F3")
        };

        _context.Preferences.Add(preference);
        _context.Categories.AddRange(defaultCategories.Select(x => new Category(
            name: x.name,
            color: x.color,
            userId: user.Id,
            isAutomaticInput: true,
            maxValueMonthly: null)));

        return UserResult.Create(user, preference);
    }

    public UserResult Update(User user) {
        var preference = GetPreference(user.Id);
        _context.Users.Update(user);
        return UserResult.Create(user, preference);
    }

    public UserResult UpdatePreference(User user, Preference preference)
    {
        _context.Preferences.Update(preference);
        return UserResult.Create(user, preference);
    } 

    public Preference GetPreference(Guid userId) =>
        _context.Preferences.FirstOrDefault(x => x.UserId.Equals(userId));
}