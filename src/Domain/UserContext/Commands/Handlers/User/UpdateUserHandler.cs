using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Domain.UserContext.Services;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Flunt.Validations;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class UpdateUserHanler : Handler<UpdateUserCommand, UpdateUserCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly UpdateUserCommandResult _commandResult;

    public UpdateUserHanler(IUserRepository userRepository, IUow uow)
    {
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<UpdateUserCommandResult> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var user = _userRepository.Get(command.Id);
        if (user is null)
        {
            AddNotification(nameof(User), $"Usuário informado não encontrado ({command.Id})");
            return Incomplete();
        }

        if (!AuthService.ValidatePassword(command.Password, user.Password))
        {
            AddNotification(nameof(command.Password), $"Senha informada não corresponde");
            return Incomplete();
        }

        user.Update(command.Name, command.Surname, command.Age, command.Sex);
        AddNotifications(user);
        if (Invalid) return Incomplete();

        _commandResult.User = _userRepository.Update(user);
        _uow.Commit();

        return Complete(_commandResult.User);
    }
}
