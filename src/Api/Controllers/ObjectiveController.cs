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

public class ObjectiveController : Controller
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly IMediator _mediator;

    public ObjectiveController(IObjectiveRepository objectiveRepository, IMediator mediator)
    {
        _objectiveRepository = objectiveRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Método que obtem os objetivos de um usuário
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <returns>Objetivos</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("v1/users/{userId:guid}/objectives")]
    public IActionResult GetObjectives(Guid userId) =>
        Ok(_objectiveRepository.GetObjectives(userId));

    /// <summary>
    /// Método que adiciona um objetivo
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações do objetivo</param>
    /// <returns>Operação</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/{userId:guid}/objectives")]
    public async Task<IActionResult> PostObjective(Guid userId, [FromBody] CreateObjectiveCommand command)
    {
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult, HttpStatusCode.Created);
    }
}