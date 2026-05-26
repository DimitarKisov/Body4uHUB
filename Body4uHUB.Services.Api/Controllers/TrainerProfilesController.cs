using Body4uHUB.Services.Api.Extensions;
using Body4uHUB.Services.Api.Models.Reviews;
using Body4uHUB.Services.Api.Models.ServiceOfferings;
using Body4uHUB.Services.Api.Models.TrainerProfiles;
using Body4uHUB.Services.Application.Commands.Review.Add;
using Body4uHUB.Services.Application.Commands.ServiceOffering.Activate;
using Body4uHUB.Services.Application.Commands.ServiceOffering.Add;
using Body4uHUB.Services.Application.Commands.ServiceOffering.Deactivate;
using Body4uHUB.Services.Application.Commands.ServiceOffering.Update;
using Body4uHUB.Services.Application.Commands.TrainerProfile.Update;
using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Queries.ServiceOffering.GetServiceOfferingsByTrainer;
using Body4uHUB.Services.Application.Queries.TrainerProfile.GetAllActiveTrainers;
using Body4uHUB.Services.Application.Queries.TrainerProfile.GetTrainerProfile;
using Body4uHUB.Shared.Api;
using Body4uHUB.Shared.Application;
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
            var result = await Mediator.Send(new GetAllActiveTrainersQuery(skip, take));
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
            var result = await Mediator.Send(new GetTrainerProfileByIdQuery(id));
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
        public async Task<IActionResult> UpdateTrainerProfile(Guid id, [FromBody] UpdateTrainerProfileRequest request)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var command = new UpdateTrainerProfileCommand(id, request.Bio, request.YearsOfExperience, authContext);
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        #endregion

        #region Service Offerings

        /// <summary>
        /// Activate a service offering
        /// </summary>
        [HttpPost("{trainerId}/services/{serviceId}/activate")]
        [Authorize(Policy= "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ActivateServiceOffering(Guid trainerId, int serviceId)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var result = await Mediator.Send(new ActivateServiceOfferingCommand(serviceId, trainerId, authContext));
            return HandleResult(result);
        }

        /// <summary>
        /// Add a review for a service offering
        /// </summary>
        [HttpPost("{orderId}/reviews")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddReview([FromRoute] int orderId, [FromBody] AddReviewRequest request)
        {
            var command = new AddReviewCommand(orderId, request.Rating, request.Comment, User.GetUserId());
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new service offering
        /// </summary>
        [HttpPost("{trainerId}/services")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(typeof(AddServiceOfferingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateServiceOffering(Guid trainerId, [FromBody] AddServiceOfferingRequest request)
        {
            var command = new AddServiceOfferingCommand(
                trainerId,
                request.Name,
                request.Description,
                request.Price,
                request.Currency,
                request.DurationMinutes,
                request.ServiceType,
                request.MaxParticipants,
                request.IsOnline,
                request.StartDate,
                request.EndDate);

            var result = await Mediator.Send(command);
            return HandleCreatedResult(result, response => response);
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
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var result = await Mediator.Send(new DeactivateServiceOfferingCommand(serviceId, trainerId, authContext));
            return HandleResult(result);
        }

        /// <summary>
        /// Get all service offerings for a trainer
        /// </summary>
        [HttpGet("{trainerId}/services")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ServiceOfferingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetServiceOfferings([FromRoute] Guid trainerId, [FromQuery] int skip, [FromQuery] int take)
        {
            var result = await Mediator.Send(new GetServiceOfferingsByTrainerQuery(trainerId, skip, take));
            return HandleResult(result);
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
        public async Task<IActionResult> UpdateServiceOffering(Guid trainerId, int serviceId, [FromBody] UpdateServiceOfferingRequest request)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var command = new UpdateServiceOfferingCommand(
                serviceId,
                trainerId,
                request.Name,
                request.Description,
                request.Price,
                request.Currency,
                request.DurationMinutes,
                request.ServiceType,
                authContext);

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        #endregion
    }
}
