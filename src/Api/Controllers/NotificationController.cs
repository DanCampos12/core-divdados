using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Core.Divdados.Api.Controllers;

public class NotificationController : Controller
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Método que processa as notificações do usuário
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <returns>Notificações</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProcessNotificationCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/{userId:guid}/notifications/process")]
    public async Task<IActionResult> Process(Guid userId)
    {
        var command = new ProcessNotificationCommand
        {
            UserId = userId,
        };
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que marca notificação como lida
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações da notificação</param>
    /// <returns>Evento</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UpdateReadNotificationCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{userId:guid}/notifications/update-read")]
    public async Task<IActionResult> UpdateRead(Guid userId, [FromBody] UpdateReadNotificationCommand command)
    {
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que marca notificação como removida
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações da notificação</param>
    /// <returns>Evento</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RemoveNotificationCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{userId:guid}/notifications/remove")]
    public async Task<IActionResult> Remove(Guid userId, [FromBody] RemoveNotificationCommand command)
    {
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }
}