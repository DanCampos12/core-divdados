using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Core.Divdados.Api.Controllers;

public class EventController : Controller
{
    private readonly IEventRepository _eventRepository;
    private readonly IMediator _mediator;

    public EventController(IEventRepository eventRepository, IMediator mediator)
    {
        _eventRepository = eventRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Método que obtem os eventos de um usuário
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <returns>Eventos</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("v1/users/{userId:guid}/events")]
    public IActionResult GetOperations(Guid userId) =>
        Ok(_eventRepository.GetEvents(userId));

    /// <summary>
    /// Método que adiciona um Evento
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações do Evento</param>
    /// <returns>Evento</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/{userId:guid}/events")]
    public async Task<IActionResult> PostEvent(Guid userId, [FromBody] CreateEventCommand command)
    {
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult, HttpStatusCode.Created);
    }

    /// <summary>
    /// Método que atualiza um evento
    /// </summary>
    /// <param name="id">Id do evento</param>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações do evento</param>
    /// <returns>Evento</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{userId:guid}/events/{id:guid}")]
    public async Task<IActionResult> PutEvent(Guid id, Guid userId, [FromBody] UpdateEventCommand command)
    {
        command.Id = id;
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que remove um evento
    /// </summary>
    /// <param name="id">Id do evento</param>
    /// <param name="userId">Id do usuário</param>
    /// <returns>Id do evento</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("v1/users/{userId:guid}/events/{id:guid}")]
    public async Task<IActionResult> DeleteEvent(Guid id, Guid userId)
    {
        var command = new DeleteEventCommand
        {
            Id = id,
            UserId = userId
        };
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }
}