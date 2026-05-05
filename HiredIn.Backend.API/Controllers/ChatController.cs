using HiredIn.Backend.API.Common;
using HiredIn.Backend.Application.Services.Chat;
using HiredIn.Backend.Application.Services.Message;
using HiredIn.Backend.Contracts.DTOs.ChatDTOs;
using HiredIn.Backend.Contracts.MessagesDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiredIn.Backend.API.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateChat(
            [FromBody] ChatCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var chatId = await Mediator.Send(
                new CreateChatCommand(dto),
                cancellationToken);

            return Ok(chatId);
        }

        [HttpGet]
        public async Task<ActionResult<List<ChatReadDTO>>> GetMyChats(
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                new GetMyChatsQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{chatId:guid}")]
        public async Task<ActionResult<ChatReadDTO>> GetChatById(
            Guid chatId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                new GetChatByIdQuery(chatId),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{chatId:guid}/messages")]
        public async Task<ActionResult<List<MessageReadDTO>>> GetMessagesByChatId(
            Guid chatId,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(
                new GetMessagesByChatIdQuery(chatId),
                cancellationToken);

            return Ok(result);
        }
    }
}