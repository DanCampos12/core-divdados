using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Services;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Flunt.Validations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class CreateCategoryHandler : Handler<CreateCategoryCommand, CreateCategoryCommandResult>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly CreateCategoryCommandResult _commandResult;

    public CreateCategoryHandler(
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<CreateCategoryCommandResult> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var category = new Category(
            name: command.Name,
            color: command.Color,
            userId: command.UserId);
        AddNotifications(category);
        if (Invalid) return Incomplete();

        Validate(category.UserId);
        if (Invalid) return Incomplete();

        _commandResult.Category = _categoryRepository.Add(category);
        _uow.Commit();

        return Complete(_commandResult.Category);
    }

    private void Validate(Guid userId)
    {
        AddNotifications(new Contract()
            .Requires()
            .IsTrue(_userRepository.Get(userId) is null,
                     nameof(User), $"Usuário inexistente"));
    }
}
