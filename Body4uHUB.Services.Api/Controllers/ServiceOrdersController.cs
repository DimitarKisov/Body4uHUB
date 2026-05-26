using Body4uHUB.Services.Api.Extensions;
using Body4uHUB.Services.Api.Models.ServiceOrders;
using Body4uHUB.Services.Application.Commands.ServiceOrders.Cancel;
using Body4uHUB.Services.Application.Commands.ServiceOrders.Complete;
using Body4uHUB.Services.Application.Commands.ServiceOrders.Confirm;
using Body4uHUB.Services.Application.Commands.ServiceOrders.Create;
using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Queries.ServiceOrders.GetServiceOrderByClients;
using Body4uHUB.Services.Application.Queries.ServiceOrders.GetServiceOrderById;
using Body4uHUB.Shared.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Body4uHUB.Services.Api.Controllers
{
    [Route("api/service-orders")]
    public class ServiceOrdersController : ApiController
    {
        /// <summary>
        /// Cancels a service order
        /// </summary>
        [HttpPut("{id}/cancel")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CancelServiceOrder(int id)
        {
            var result = await Mediator.Send(new CancelServiceOrderCommand(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Completes a service order
        /// </summary>
        [HttpPut("{id}/complete")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CompleteServiceOrder(int id)
        {
            var result = await Mediator.Send(new CompleteServiceOrderCommand(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Confirms a service order
        /// </summary>
        [HttpPut("{id}/confirm")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ConfirmServiceOrder(int id)
        {
            var result = await Mediator.Send(new ConfirmServiceOrderCommand(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Creates a new service order
        /// </summary>
        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(typeof(CreateServiceOrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateServiceOrder([FromBody] CreateServiceOrderRequest request)
        {
            var command = new CreateServiceOrderCommand(
                User.GetUserId(),
                request.TrainerId,
                request.ServiceOfferingId,
                request.Notes);

            var result = await Mediator.Send(command);
            return HandleCreatedResult(result, response => response);
        }

        /// <summary>
        /// Gets service orders by client ID
        /// </summary>
        [HttpGet("by-client/{clientId}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<ServiceOrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetServiceOrdersByClient(Guid clientId)
        {
            var result = await Mediator.Send(new GetOrdersByClientQuery(clientId));
            return HandleResult(result);
        }

        /// <summary>
        /// Gets a service order by its ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ServiceOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetServiceOrderById(int id)
        {
            var result = await Mediator.Send(new GetServiceOrderByIdQuery(id));
            return HandleResult(result);
        }
    }
}
