using Body4uHUB.Services.Api.Extensions;
using Body4uHUB.Services.Application.Commands.ServiceOffering.Add;
using Body4uHUB.Services.Application.Commands.ServiceOffering.Deactivate;
using Body4uHUB.Services.Application.Commands.ServiceOffering.Update;
using Body4uHUB.Services.Application.Commands.TrainerProfile.Update;
using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Queries.ServiceOffering.GetServiceOfferingsByTrainer;
using Body4uHUB.Services.Application.Queries.TrainerProfile.GetAllActiveTrainers;
using Body4uHUB.Services.Application.Queries.TrainerProfile.GetTrainerProfile;
using Body4uHUB.Shared.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Body4uHUB.Services.Api.Controllers
{
    [Route("api/trainer-profiles")]
    public class TrainerProfilesController : ApiController
    {
        #region Trainer Profile Management

        /// <summary>
        /// Get all trainer profiles with pagination and filters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TrainerProfileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActiveTrainerProfiles([FromQuery] int skip, [FromQuery] int take)
        {
            var result = await Mediator.Send(new GetAllActiveTrainersQuery { Skip = skip, Take = take });
            return HandleResult(result);
        }

        /// <summary>
        /// Get trainer profile by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TrainerProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetTrainerProfile(Guid id)
        {
            var result = await Mediator.Send(new GetTrainerProfileByIdQuery { TrainerId = id });
            return HandleResult(result);
        }

        /// <summary>
        /// Update trainer profile
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateTrainerProfile(Guid id, [FromBody] UpdateTrainerProfileCommand command)
        {
            command.Id = id;
            command.CurrentUserId = User.GetUserId();
            command.IsAdmin = User.IsAdmin();

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        #endregion

        #region Service Offerings

        /// <summary>
        /// Get all service offerings for a trainer
        /// </summary>
        [HttpGet("{trainerId}/services")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ServiceOfferingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetServiceOfferings(Guid trainerId)
        {
            var result = await Mediator.Send(new GetServiceOfferingsByTrainerQuery { TrainerId = trainerId });
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new service offering (Trainer own profile only)
        /// </summary>
        [HttpPost("{trainerId}/services")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateServiceOffering(Guid trainerId, [FromBody] AddServiceOfferingCommand command)
        {
            command.TrainerId = trainerId;

            var result = await Mediator.Send(command);
            return HandleResult(result, id => new { serviceId = id });
        }

        /// <summary>
        /// Update service offering
        /// </summary>
        [HttpPut("{trainerId}/services/{serviceId}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateServiceOffering(
            Guid trainerId,
            int serviceId,
            [FromBody] UpdateServiceOfferingCommand command)
        {
            command.Id = serviceId;
            command.TrainerId = trainerId;
            command.CurrentUserId = User.GetUserId();
            command.IsAdmin = User.IsAdmin();

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Delete service offering
        /// </summary>
        [HttpDelete("{trainerId}/services/{serviceId}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteServiceOffering(Guid trainerId, int serviceId)
        {
            var command = new DeactivateServiceOfferingCommand
            {
                Id = serviceId,
                TrainerId = trainerId,
                CurrentUserId = User.GetUserId(),
                IsAdmin = User.IsAdmin()
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        #endregion
    }
}
