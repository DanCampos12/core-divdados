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

    public IEnumerable<CategoryResult> GetCategories(Guid userId)
    {
        var userOperations = _context.Operations.Where(x => x.UserId.Equals(userId) && x.Effected).ToArray();
        return GetCategoriesQuery(userId, userOperations).ToArray();
    }

    public CategoryResult Add(Category category) {
        _context.Categories.Add(category);
        return CategoryResult.Create(category, 0.0M);
    }

    public CategoryResult Update(Category category) {
        _context.Categories.Update(category);
        return CategoryResult.Create(category, 0.0M);
    }

    public Guid Delete(Category category)
    {
        _context.Categories.Remove(category);
        return category.Id;
    }

    private IQueryable<CategoryResult> GetCategoriesQuery(Guid userId, Operation[] operations) =>
        from category in _context.Categories
        where category.UserId.Equals(userId)
        orderby category.Name
        select CategoryResult.Create(category, GetCategoryAllocation(category, operations));
        

    private static decimal GetCategoryAllocation (Category category, Operation[] operations)
    {
        var categoryCount = operations.Count(x => x.CategoryId.Equals(category.Id));
        return !operations.Any() ? 0.0M : ((decimal)categoryCount / (decimal)operations.Length);
    }
}