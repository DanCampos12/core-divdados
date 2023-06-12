using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class UpdateCategoryHandler : Handler<UpdateCategoryCommand, UpdateCategoryCommandResult>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly UpdateCategoryCommandResult _commandResult;

    public UpdateCategoryHandler(
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<UpdateCategoryCommandResult> Handle(UpdateCategoryCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var user = _userRepository.Get(command.UserId);
        if (user is null)
        {
            AddNotification(nameof(User), $"Usuário informado não encontrado ({command.Id})");
            return Incomplete();
        }

        var category = _categoryRepository.GetCategory(command.Id, command.UserId);
        if (category is null)
        {
            AddNotification(nameof(command.Id), $"Categoria não encontrada");
            return Incomplete();
        }

        category.Update(command.Name, command.Color);
        AddNotifications(category);
        if (Invalid) return Incomplete();

        _commandResult.Category = _categoryRepository.Update(category);
        _uow.Commit();

        return Complete(_commandResult.Category);
    }
}
