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
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ObjectiveResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("v1/users/{userId:guid}/objectives")]
    public IActionResult GetObjectives(Guid userId) =>
        Ok(_objectiveRepository.GetObjectives(userId));

    /// <summary>
    /// Método que adiciona um objetivo
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações do objetivo</param>
    /// <returns>Objetivo</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateObjectiveCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/{userId:guid}/objectives")]
    public async Task<IActionResult> PostObjective(Guid userId, [FromBody] CreateObjectiveCommand command)
    {
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult, HttpStatusCode.Created);
    }

    /// <summary>
    /// Método que atualiza um objetivo
    /// </summary>
    /// <param name="id">Id do objetivo</param>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações do objetivo</param>
    /// <returns>Objetivo</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateObjectiveCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{userId:guid}/objectives/{id:guid}")]
    public async Task<IActionResult> PutObjective(Guid id, Guid userId, [FromBody] UpdateObjectiveCommand command)
    {
        command.Id = id;
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que remove um objetivo
    /// </summary>
    /// <param name="id">Id do objetivo</param>
    /// <param name="userId">Id do usuário</param>
    /// <returns>Id do objetivo</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("v1/users/{userId:guid}/objectives/{id:guid}")]
    public async Task<IActionResult> DeleteOperation(Guid id, Guid userId)
    {
        var command = new DeleteObjectiveCommand
        {
            Id = id,
            UserId = userId
        };
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que processa os objetivos
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações do objetivo</param>
    /// <returns>Objetivos processados</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessObjectivesCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/{userId:guid}/objectives/process")]
    public async Task<IActionResult> ProcessObjectives(Guid userId)
    {
        var command = new ProcessObjectivesCommand
        {
            UserId = userId
        };
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que atualiza a ordem dos objetivos
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações da nova ordem dos objetivos</param>
    /// <returns>Objetivo</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReorderObjectivesCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{userId:guid}/objectives/reorder")]
    public async Task<IActionResult> PutObjectivesOrder(Guid userId, [FromBody] ReorderObjectivesCommand command)
    {
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que atualiza um objetivo
    /// </summary>
    /// <param name="id">Id do objetivo</param>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações do objetivo</param>
    /// <returns>Objetivo</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompleteObjectiveCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{userId:guid}/objectives/{id:guid}/complete")]
    public async Task<IActionResult> CompleteObjective(Guid id, Guid userId, [FromQuery] bool shouldLaunchOperation)
    {
        var command = new CompleteObjectiveCommand
        {
            Id = id,
            UserId = userId,
            ShouldLaunchOperation = shouldLaunchOperation
        };
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }
}