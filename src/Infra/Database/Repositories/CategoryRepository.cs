using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Infra.SQL.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Divdados.Infra.SQL.Repositories;

public class CategoryRepository : ICategoryRepository
{
    public UserDataContext _context;

    public CategoryRepository(UserDataContext context) => _context = context;

    public Category GetCategory(Guid id, Guid userId) => _context.Categories
        .Where(category => category.Id.Equals(id) && category.UserId.Equals(userId))
        .FirstOrDefault();

    public IEnumerable<CategoryResult> GetCategories(Guid userId) =>
        GetCategoriesQuery(userId).ToArray();

    public CategoryResult Add(Category category) {
        _context.Categories.Add(category);
        return CategoryResult.Create(category);
    }

    public CategoryResult Update(Category category) {
        _context.Categories.Update(category);
        return CategoryResult.Create(category);
    }

    public Guid Delete(Category category)
    {
        _context.Categories.Remove(category);
        return category.Id;
    }

    private IQueryable<CategoryResult> GetCategoriesQuery(Guid userId) =>
        from category in _context.Categories
        where category.UserId.Equals(userId)
        select new CategoryResult
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color,
            UserId = category.UserId
        };
}