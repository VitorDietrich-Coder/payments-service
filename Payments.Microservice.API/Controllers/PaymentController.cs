using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Microservice.API.Swagger.Attributes;
using Payments.Microservice.Application.DTO;
using Payments.Microservice.Application.Queries;
using Swashbuckle.AspNetCore.Annotations;

namespace Payments.Microservice.API.Controllers;

/// <summary>
/// Manages payments-related operations such as listing.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class PaymentsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
 

    /// <summary>
    /// Gets a game by its ID.
    /// </summary>
    /// <param name="id">Game ID.</param>
    /// <returns>Game details if found.</returns>
    [HttpGet("{id:int:min(1)}")]
    [SwaggerOperation(
        Summary = "Get Payments by user ID",
        Description = "Returns Payments details for the given ID. Returns 404 if the game doesn't exist."
    )]

    [SwaggerResponseProfile("Games.Get")]
    public async Task<IEnumerable<PaymentDto>> GetAsync(Guid id)
    {
        return await Mediator.Send(new GetPaymentsQuery(id));
    }
}
