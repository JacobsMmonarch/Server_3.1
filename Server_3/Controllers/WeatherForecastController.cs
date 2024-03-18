using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;

namespace Server_3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileTransferController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("File uploaded successfully");
        }

        [HttpGet("download/{fileName}")]
        public IActionResult Download(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found");

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".xlsx", "table/xlsx" }
            };
        }
    }
}
