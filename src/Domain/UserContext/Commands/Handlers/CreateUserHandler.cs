using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Services;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Flunt.Validations;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class CreateUserHandler : Handler<CreateUserCommand, CreateUserCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly CreateUserCommandResult _commandResult;

    public CreateUserHandler(IUserRepository userRepository, IUow uow)
    {
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<CreateUserCommandResult> Handle(CreateUserCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var user = new User(
            name: command.Name,
            surname: command.Surname,
            email: command.Email,
            password: AuthService.EncryptPassword(command.Password),
            age: command.Age,
            sex: command.Sex);
        AddNotifications(user);
        if (Invalid) return Incomplete();

        Validate(user);
        if (Invalid) return Incomplete();

        _commandResult.User = _userRepository.Add(user);
        _uow.Commit();

        return Complete(_commandResult.User);
    }

    private void Validate(User user)
    {
        AddNotifications(new Contract()
            .Requires()
            .IsFalse(_userRepository.GetByEmail(user.Email) is not null,
                     nameof(User), $"Email ({user.Email}) já cadastrado na base"));
    }
}
