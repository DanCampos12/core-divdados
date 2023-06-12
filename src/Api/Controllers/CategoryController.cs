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

public class CategoryController : Controller
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMediator _mediator;

    public CategoryController(ICategoryRepository categoryRepository, IMediator mediator)
    {
        _categoryRepository = categoryRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Método que obtem as categorias de um usuário
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <returns>Categorias</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("v1/users/{userId:guid}/categories")]
    public IActionResult GetCategories(Guid userId) =>
        Ok(_categoryRepository.GetCategories(userId));

    /// <summary>
    /// Método que adiciona uma categoria
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações da categoria</param>
    /// <returns>Categoria</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/{userId:guid}/categories")]
    public async Task<IActionResult> PostCategory(Guid userId, [FromBody] CreateCategoryCommand command)
    {
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult, HttpStatusCode.Created);
    }

    /// <summary>
    /// Método que atualiza uma categoria
    /// </summary>
    /// <param name="id">Id da categoria</param>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações da categoria</param>
    /// <returns>Categoria</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{userId:guid}/categories/{id:guid}")]
    public async Task<IActionResult> PutCategory(Guid id, Guid userId, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que remove uma categoria
    /// </summary>
    /// <param name="id">Id da categoria</param>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações da categoria</param>
    /// <returns>Id da categoria</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("v1/users/{userId:guid}/categories/{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id, Guid userId)
    {
        var command = new DeleteCategoryCommand
        {
            Id = id,
            UserId = userId
        };
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }
}