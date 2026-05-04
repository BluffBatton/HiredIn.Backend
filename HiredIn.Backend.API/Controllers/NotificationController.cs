using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Notification;
using HiredIn.Backend.Contracts.DTOs.NotificationDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize]
    public class NotificationController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<List<NotificationReadDTO>>> GetMyNotifications(
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                new GetMyNotificationsQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{notificationId:guid}")]
        public async Task<ActionResult<NotificationReadDTO>> GetNotificationById(
            Guid notificationId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                new GetNotificationByIdQuery(notificationId),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{notificationId:guid}")]
        public async Task<IActionResult> MarkNotificationAsRead(
            Guid notificationId,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(
                new MarkNotificationAsReadCommand(notificationId),
                cancellationToken);

            return NoContent();
        }

        [HttpPatch]
        public async Task<IActionResult> MarkAllNotificationsAsRead(
            CancellationToken cancellationToken)
        {
            await Mediator.Send(
                new MarkAllNotificationsAsReadCommand(),
                cancellationToken);

            return NoContent();
        }

        [HttpDelete("{notificationId:guid}")]
        public async Task<IActionResult> DeleteNotification(
            Guid notificationId,
            CancellationToken cancellationToken)
        {
            await Mediator.Send(
                new DeleteNotificationCommand(notificationId),
                cancellationToken);

            return NoContent();
        }
    }
}