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

public sealed class CreateEventHandler : Handler<CreateEventCommand, CreateEventCommandResult>
{
    private readonly IEventRepository _eventRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly CreateEventCommandResult _commandResult;

    public CreateEventHandler(
        IEventRepository eventRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _eventRepository = eventRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<CreateEventCommandResult> Handle(CreateEventCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var operation = new Event(
            value: command.Value,
            type: command.Type,
            description: command.Description,
            initialDate: command.InitialDate,
            finalDate: command.FinalDate,
            period: command.Period,
            userId: command.UserId,
            categoryId: command.CategoryId);
        AddNotifications(operation);
        if (Invalid) return Incomplete();

        Validate(operation.UserId, operation.CategoryId);
        if (Invalid) return Incomplete();

        _commandResult.Event = _eventRepository.Add(operation);
        _uow.Commit();

        return Complete(_commandResult.Event);
    }

    private void Validate(Guid userId, Guid categoryId)
    {
        AddNotifications(new Contract()
            .Requires()
            .IsFalse(_userRepository.Get(userId) is null, nameof(User), $"Usuário inexistente")
            .IsFalse(_categoryRepository.GetCategory(categoryId, userId) is null, nameof(User), $"Categoria inexistente"));
    }
}
