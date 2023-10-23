using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        // POST: {apibaseurl}/api/images
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file,
            [FromForm] string fileName,
            [FromForm] string title)
        {
            ValidateFileUpload(file);

            if (ModelState.IsValid)
            {
                // file upload
                var blogImage = new BlogImage
                {
                    FileName = fileName,
                    Title = title,
                    DateCreated = DateTime.Now,
                    FileExtension = Path.GetExtension(file.FileName).ToLower()
                };

                await imageRepository.Upload(file, blogImage);

                // convert domain model to dto
                var response = new BlogImageDTO
                {
                    Id = blogImage.Id,
                    Title = blogImage.Title,
                    FileExtension = blogImage.FileExtension,
                    FileName = blogImage.FileName,
                    DateCreated = blogImage.DateCreated,
                    Url = blogImage.Url
                };

                return Ok(response);

            }

            return BadRequest(ModelState);
        }

        // GET: {apibaseurl}/api/Images
        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            // call image repository to get all images
            var images = await imageRepository.GetAll();

            // convert domain model to dto
            var response = new List<BlogImageDTO>();
            foreach (var image in images)
            {
                response.Add(new BlogImageDTO
                {
                    Id = image.Id,
                    Title = image.Title,
                    FileExtension = image.FileExtension,
                    FileName = image.FileName,
                    DateCreated = image.DateCreated,
                    Url = image.Url
                });
            }

            return Ok(response);
        }

        private void ValidateFileUpload(IFormFile file)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsuppoted file fomat");
            }

            if (file.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size cannot be more than 10MB");
            }

        }
    }
}
