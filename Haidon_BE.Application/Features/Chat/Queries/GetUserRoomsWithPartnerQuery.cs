using Haidon_BE.Application.Features.Chat.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haidon_BE.Application.Features.Chat.Queries
{
    public class GetUserRoomsWithPartnerQuery : IRequest<List<RoomWithParnerDto>>
    {
        public Guid UserId { get; set; }
    }
}
