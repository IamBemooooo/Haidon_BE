using Haidon_BE.Application.Features.UserProfiles.Dtos;
using Haidon_BE.Domain.Entities;
using Haidon_BE.Domain.Enums;
using Haidon_BE.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;

namespace Haidon_BE.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private static readonly string[] ImageExtensions =
        {
            ".jpg", ".jpeg", ".png", ".tif", ".tiff", ".heic"
        };

        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly ApplicationDbContext _dbContext;

        public FileController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("upload-images")]
        public async Task<ActionResult<List<UploadImageResultDto>>> UploadImages(
            [FromForm] UploadIamgeRequest request)
        {
            if (request.Files == null || request.Files.Count == 0)
                return BadRequest("Không có file upload");

            var results = new List<UploadImageResultDto>();
            var uploadRoot = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "avatars");
            Directory.CreateDirectory(uploadRoot);

            foreach (var imageReq in request.Files)
            {
                var file = imageReq.File;
                if (file == null || file.Length == 0)
                    continue;
                if (file.Length > MaxFileSize)
                    return BadRequest("Ảnh vượt quá 10MB");
                if (!file.ContentType.StartsWith("image/"))
                    return BadRequest("File không phải hình ảnh");

                // Xác định định dạng thực tế bằng ImageSharp
                IImageFormat? actualFormat = null;
                try
                {
                    using var image = await Image.LoadAsync(file.OpenReadStream());
                    actualFormat = image.Metadata.DecodedImageFormat;
                }
                catch
                {
                    return BadRequest("File không phải hình ảnh hợp lệ hoặc định dạng không hỗ trợ");
                }
                if (actualFormat == null)
                    return BadRequest("Không xác định được định dạng ảnh");
                var extension = $".{actualFormat.FileExtensions.FirstOrDefault()}";
                if (!ImageExtensions.Contains(extension))
                    return BadRequest("Định dạng ảnh không được hỗ trợ");

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadRoot, fileName);

                if (extension == ".heic")
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                }
                else
                {
                    file.OpenReadStream().Position = 0; // reset lại stream
                    using var image = await Image.LoadAsync(file.OpenReadStream());
                    await image.SaveAsync(filePath, GetEncoder(extension, 85));
                }

                // Xóa bản ghi UserMedia cũ cùng userId và order
                var oldMedia = _dbContext.UserMedias.Where(m => m.UserId == request.UserId && m.Order == imageReq.order);
                if (oldMedia.Count() > 0)
                {
                    _dbContext.UserMedias.RemoveRange(oldMedia);
                }

                var userMedia = new UserMedia
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Url = $"/uploads/avatars/{fileName}",
                    Type = (MediaType)imageReq.type,
                    Order = imageReq.order,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.UserMedias.Add(userMedia);

                results.Add(new UploadImageResultDto
                {
                    Id = userMedia.Id,
                    UserId = userMedia.UserId,
                    Url = userMedia.Url,
                    Type = userMedia.Type,
                    Order = userMedia.Order
                });
            }
            await _dbContext.SaveChangesAsync();
            return Ok(results);
        }

        private static IImageEncoder GetEncoder(string ext, int quality)
        {
            return ext switch
            {
                ".png" => new PngEncoder(),
                ".tif" or ".tiff" => new TiffEncoder(),
                ".webp" => new WebpEncoder { Quality = quality },
                _ => new JpegEncoder { Quality = quality } // jpg, jpeg
            };
        }
    }
}
