using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("uploadFile")]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromForm] FileRequest fileRequest)
        {
            // Kiểm tra xem file có phải là hình ảnh hay không
            if (!_fileService.IsImageFile(fileRequest.imageFile.FileName))
            {
                BadRequest("Only image files are allowed.");
            }
            var imageUrl = await _fileService.Upload(fileRequest);
            return Ok(new { imageUrl });

        }

        [HttpGet]
        public async Task<IActionResult> DownloadImage(string name)
        {
            var imageFileStream = await _fileService.Get(name);
            string fileType = "jpeg";
            if (name.Contains("png"))
            {
                fileType = "png";
            }
            return File(imageFileStream, $"image/{fileType}", $"blobfile.{fileType}");
        }
    }
}
