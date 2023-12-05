using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class UpdatePreferenceHandler : Handler<UpdatePreferenceCommand, UpdatePreferenceCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly UpdatePreferenceCommandResult _commandResult;

    public UpdatePreferenceHandler(IUserRepository userRepository, IUow uow)
    {
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<UpdatePreferenceCommandResult> Handle(UpdatePreferenceCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var user = _userRepository.Get(command.UserId);
        if (user is null)
        {
            AddNotification(nameof(User), $"Usuário informado não encontrado ({command.UserId})");
            return Incomplete();
        }

        var preference = _userRepository.GetPreference(command.UserId);
        if (preference is null)
        {
            AddNotification(nameof(User), $"Preferências de usuário não encontrada ({command.UserId})");
            return Incomplete();
        }

        preference.Update(command.Dark, command.DisplayValues);
        AddNotifications(preference);
        if (Invalid) return Incomplete();

        _commandResult.User = _userRepository.UpdatePreference(user, preference);
        _uow.Commit();

        return Complete(_commandResult.User);
    }
}
