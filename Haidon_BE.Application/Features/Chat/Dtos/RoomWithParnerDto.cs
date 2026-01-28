using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haidon_BE.Application.Features.Chat.Dtos
{
    public class RoomWithParnerDto
    {
        public Guid RoomId { get; set; }
        public Guid PartnerUserId { get; set; }
    }
}
