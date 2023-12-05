using Core.Divdados.Domain.UserContext.Results;
using System;

namespace Core.Divdados.Domain.UserContext.Repositories;

public interface IOutputDataRepository
{
    public OutputDataResult GetOutputData(Guid userId, DateTime date);
}