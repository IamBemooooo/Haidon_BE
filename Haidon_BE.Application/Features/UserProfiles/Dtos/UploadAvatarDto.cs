using System;
using Haidon_BE.Domain.Enums;

namespace Haidon_BE.Application.Features.UserProfiles.Dtos
{
    public class UploadImageResultDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Url { get; set; } = string.Empty;
        public MediaType Type { get; set; }
        public int Order { get; set; }
    }
}
