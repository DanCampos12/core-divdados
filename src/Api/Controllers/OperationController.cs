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

public class OperationController : Controller
{
    private readonly IOperationRepository _operationRepository;
    private readonly IMediator _mediator;

    public OperationController(IOperationRepository operationRepository, IMediator mediator)
    {
        _operationRepository = operationRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Método que obtem as operações de um usuário
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <returns>Operações</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OperationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("v1/users/{userId:guid}/operations")]
    public IActionResult GetOperations(Guid userId) =>
        Ok(_operationRepository.GetOperations(userId));

    /// <summary>
    /// Método que adiciona uma Operação
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações da operação</param>
    /// <returns>Operação</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateOperationCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/{userId:guid}/operations")]
    public async Task<IActionResult> PostOperation(Guid userId, [FromBody] CreateOperationCommand command)
    {
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult, HttpStatusCode.Created);
    }

    /// <summary>
    /// Método que atualiza uma operação
    /// </summary>
    /// <param name="id">Id da operação</param>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações da operação</param>
    /// <returns>Categoria</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateOperationCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{userId:guid}/operations/{id:guid}")]
    public async Task<IActionResult> PutOperation(Guid id, Guid userId, [FromBody] UpdateOperationCommand command)
    {
        command.Id = id;
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que remove uma operação
    /// </summary>
    /// <param name="id">Id da operação</param>
    /// <param name="userId">Id do usuário</param>
    /// <returns>Id da operação</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("v1/users/{userId:guid}/operations/{id:guid}")]
    public async Task<IActionResult> DeleteOperation(Guid id, Guid userId)
    {
        var command = new DeleteOperationCommand
        {
            Id = id,
            UserId = userId
        };
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que efetua uma operação
    /// </summary>
    /// <param name="id">Id da operação</param>
    /// <param name="userId">Id do usuário</param>
    /// <returns>Categoria</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EffectOperationCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{userId:guid}/operations/{id:guid}/effect")]
    public async Task<IActionResult> EffectOperation(Guid id, Guid userId)
    {
        var command = new EffectOperationCommand
        {
            Id = id,
            UserId = userId
        };
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }
}