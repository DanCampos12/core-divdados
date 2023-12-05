using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Core.Divdados.Api.Controllers;

public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;

    public UserController(IUserRepository userRepository, IMediator mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Método que adiciona um usuário
    /// </summary>
    /// <param name="command">Informações do usuário</param>
    /// <returns>Usuário</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateUserCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users")]
    public async Task<IActionResult> PostUser([FromBody] CreateUserCommand command)
    {
        var commandResult = await _mediator.Send(command);
        return Response(commandResult, HttpStatusCode.Created);
    }

    /// <summary>
    /// Método que atualiza um usuário
    /// </summary>
    /// <param name="id">Id do usuário</param>
    /// <param name="command">Informações do usuário</param>
    /// <returns>Usuário</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateUserCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{id:guid}")]
    public async Task<IActionResult> PutUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que atualiza as preferências do usuário
    /// </summary>
    /// <param name="id">Id do usuário</param>
    /// <param name="command">Informações da preferência</param>
    /// <returns>Usuário</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatePreferenceCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{id:guid}/preferences")]
    public async Task<IActionResult> PostUser(Guid id, [FromBody] UpdatePreferenceCommand command)
    {
        command.UserId = id;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }
}