using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haidon_BE.Application.Features.UserProfiles.Dtos
{
    public class UploadIamgeRequest
    {
        public Guid UserId { get; set; }
        public List<IFormFile> Files { get; set; } = new();
    }
}
