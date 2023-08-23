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

public sealed class CreateOperationHandler : Handler<CreateOperationCommand, CreateOperationCommandResult>
{
    private readonly IOperationRepository _operationRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly CreateOperationCommandResult _commandResult;

    public CreateOperationHandler(
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

    public override Task<CreateOperationCommandResult> Handle(CreateOperationCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var operation = new Operation(
            value: command.Value,
            type: command.Type,
            description: command.Description,
            date: command.Date,
            effected: command.Effected,
            userId: command.UserId,
            categoryId: command.CategoryId,
            eventId: command.EventId);
        AddNotifications(operation);
        if (Invalid) return Incomplete();

        Validate(operation.UserId, operation.CategoryId);
        if (Invalid) return Incomplete();

        _commandResult.Operation = _operationRepository.Add(operation);
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
