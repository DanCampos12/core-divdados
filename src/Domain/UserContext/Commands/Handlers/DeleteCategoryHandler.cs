using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class DeleteCategoryHandler : Handler<DeleteCategoryCommand, DeleteCategoryCommandResult>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly DeleteCategoryCommand _commandResult;

    public DeleteCategoryHandler(
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<DeleteCategoryCommandResult> Handle(DeleteCategoryCommand command, CancellationToken ct)
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

        AddNotifications(category);
        if (Invalid) return Incomplete();

        _commandResult.Id = _categoryRepository.Delete(category);
        _uow.Commit();

        return Complete(_commandResult.Id);
    }
}
