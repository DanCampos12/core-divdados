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

public sealed class UpdateEventHandler : Handler<UpdateEventCommand, UpdateEventCommandResult>
{
    private readonly IEventRepository _eventRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly UpdateEventCommandResult _commandResult;

    public UpdateEventHandler(
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

    public override Task<UpdateEventCommandResult> Handle(UpdateEventCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var @event = _eventRepository.GetEvent(command.Id, command.UserId);
        if (@event is null)
        {
            AddNotification(nameof(command.Id), $"Evento não encontrado");
            return Incomplete();
        }

        Validate(command.UserId, command.CategoryId);
        if (Invalid) return Incomplete();

        @event.Update(command.Value, command.Description, command.CategoryId);
        AddNotifications(@event);
        if (Invalid) return Incomplete();

        _commandResult.Event = _eventRepository.Update(@event);
        _uow.Commit();

        return Complete(_commandResult.Event);
    }

    private void Validate(Guid userId, Guid categoryId)
    {
        AddNotifications(new Contract()
            .Requires()
            .IsFalse(_categoryRepository.GetCategory(categoryId, userId) is null, nameof(User), $"Categoria inexistente"));
    }
}