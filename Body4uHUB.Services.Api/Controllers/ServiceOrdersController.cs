using Body4uHUB.Services.Application.Commands.ServiceOrders.Cancel;
using Body4uHUB.Services.Application.Commands.ServiceOrders.Complete;
using Body4uHUB.Services.Application.Commands.ServiceOrders.Confirm;
using Body4uHUB.Services.Application.Commands.ServiceOrders.Create;
using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Queries.ServiceOrders.GetServiceOrderByClients;
using Body4uHUB.Services.Application.Queries.ServiceOrders.GetServiceOrderById;
using Body4uHUB.Services.Domain.ValueObjects;
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
        [HttpPost("cancel")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CancelServiceOrder(CancelServiceOrderCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Completes a service order
        /// </summary>
        [HttpPost("complete")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CompleteServiceOrder(CompleteServiceOrderCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Confirms a service order
        /// </summary>
        [HttpPost("confirm")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ConfirmServiceOrder(ConfirmServiceOrderCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Creates a new service order
        /// </summary>
        [HttpPost("create")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(typeof(ServiceOrderId), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateServiceOrder(CreateServiceOrderCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
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
            var query = new GetOrdersByClientQuery
            {
                ClientId = clientId
            };

            var result = await Mediator.Send(query);
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
            var query = new GetServiceOrderByIdQuery
            {
                Id = id
            };

            var result = await Mediator.Send(query);
            return HandleResult(result);
        }
    }
}
