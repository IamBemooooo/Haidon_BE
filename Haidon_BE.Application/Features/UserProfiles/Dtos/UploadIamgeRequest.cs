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
        public List<ImageRequest> Files { get; set; } = new();
    }

    public class ImageRequest
    {
        public IFormFile File { get; set; } = null!;
        public int order { get; set; }
        public int type { get; set; }
    }
}
