using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Results;
using System;
using System.Collections.Generic;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface ICategoryRepository
{
    Category GetCategory(Guid id, Guid userId);
    IEnumerable<CategoryResult> GetCategories(Guid userId);
    CategoryResult Add(Category category);
    CategoryResult Update(Category category);
    Guid Delete(Category category);
}