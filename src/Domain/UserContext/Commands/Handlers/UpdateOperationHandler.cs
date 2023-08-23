using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Flunt.Validations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class UpdateOperationHandler : Handler<UpdateOperationCommand, UpdateOperationCommandResult>
{
    private readonly IOperationRepository _operationRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly UpdateOperationCommandResult _commandResult;

    public UpdateOperationHandler(
        IOperationRepository operationRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _operationRepository = operationRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<UpdateOperationCommandResult> Handle(UpdateOperationCommand command, CancellationToken ct)
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

        var operation = _operationRepository.GetOperation(command.Id, command.UserId);
        if (operation is null)
        {
            AddNotification(nameof(command.Id), $"Operação não encontrada");
            return Incomplete();
        }

        Validate(command.UserId, command.CategoryId);
        if (Invalid) return Incomplete();

        operation.Update(command.Value, command.Description, command.CategoryId);
        AddNotifications(operation);
        if (Invalid) return Incomplete();

        _commandResult.Operation = _operationRepository.Update(operation);
        _uow.Commit();

        return Complete(_commandResult.Operation);
    }

    private void Validate(Guid userId, Guid categoryId)
    {
        AddNotifications(new Contract()
            .Requires()
            .IsFalse(_userRepository.Get(userId) is null, nameof(User), $"Usuário inexistente")
            .IsFalse(_categoryRepository.GetCategory(categoryId, userId) is null, nameof(User), $"Categoria inexistente"));
    }
}