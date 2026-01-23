using MediatR;
using Haidon_BE.Application.Features.Chat.Dtos;

namespace Haidon_BE.Application.Features.Chat.Queries;

public class GetMessagesQuery : IRequest<List<MessageDto>>
{
    public Guid RoomId { get; set; }
}
