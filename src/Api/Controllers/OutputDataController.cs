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

public class OutputDataControler : Controller
{
    private readonly IOutputDataRepository _outputDataRepository;
    private readonly IMediator _mediator;

    public OutputDataControler(IOutputDataRepository outputDataRepository)
        => _outputDataRepository = outputDataRepository;
   

    /// <summary>
    /// Método que obtem um overview das operações do usuário
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="date">Data</param>
    /// <returns>Overview</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OutputDataResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("v1/users/{userId:guid}/overview/{date:DateTime}")]
    public IActionResult GetCategories(Guid userId, DateTime date) =>
        Ok(_outputDataRepository.GetOutputData(userId, date));
}