using Haidon_BE.Application.Features.Chat.Commands;
using Haidon_BE.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Haidon_BE.Application.Services
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task Ping()
        {
            await Clients.All.SendAsync("Pong", "pong");
        }
    }
}
