using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haidon_BE.Application.Features.Chat.Queries
{
    public class GetActiveRoomsQuery : IRequest<List<Guid>>
    {
        public Guid UserId { get; set; }
    }
}
