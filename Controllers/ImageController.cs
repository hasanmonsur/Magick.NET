using ImageMagick;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        [HttpPost("process")]
        public async Task<IActionResult> ProcessImage(IFormFile file, int width, int height, string format)
        {
            if(file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                using var stream = file.OpenReadStream();
                using var image = new MagickImage(stream);

                // Resize the image
                image.Resize((uint)width, (uint)height);

                // Convert the image format
                if (!Enum.TryParse<MagickFormat>(format, true, out var magickFormat))
                {
                    return BadRequest("Invalid format specified");
                }

                image.Format = magickFormat;

                // Create a MemoryStream without disposing it
                var memoryStream = new MemoryStream();
                image.Write(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Return the image as a file response
                return File(memoryStream, "application/octet-stream", $"processed_image.{format.ToLower()}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing the image: {ex.Message}");
            }

        }
    }
}
