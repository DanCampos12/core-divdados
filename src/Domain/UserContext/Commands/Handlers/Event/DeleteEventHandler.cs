using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class DeleteEventHandler : Handler<DeleteEventCommand, DeleteEventCommandResult>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly DeleteEventCommandResult _commandResult;

    public DeleteEventHandler(
        IEventRepository eventRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<DeleteEventCommandResult> Handle(DeleteEventCommand command, CancellationToken ct)
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

        var @event = _eventRepository.GetEvent(command.Id, command.UserId);
        if (@event is null)
        {
            AddNotification(nameof(command.Id), $"Evento não encontrado");
            return Incomplete();
        }

        AddNotifications(@event);
        if (Invalid) return Incomplete();

        _commandResult.Id = _eventRepository.Delete(@event);
        _uow.Commit();

        return Complete(_commandResult.Id);
    }
}
