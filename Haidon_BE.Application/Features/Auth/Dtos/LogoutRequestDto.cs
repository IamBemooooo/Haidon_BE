using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haidon_BE.Application.Features.Auth.Dtos
{
    public readonly record struct LogoutRequestDto(Guid userId);
}
