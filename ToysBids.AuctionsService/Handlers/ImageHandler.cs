using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Armadar.ImageWriter;
using System.Dynamic;

namespace ToysBids.AuctionsService.Handlers
{
    public interface IImageHandler
    {
        Task<IActionResult> UploadImage(IFormFile file, string name);
    }
    public class ImageHandler : IImageHandler
    {
        private readonly IImageWriter _imageWriter;
        public ImageHandler(IImageWriter imageWriter)
        {
            _imageWriter = imageWriter;
        }

        public async Task<IActionResult> UploadImage(IFormFile file, string name)
        {
            var result = await _imageWriter.UploadImage(file, name);
            dynamic r = new ExpandoObject();
            r.message = result;

            return new ObjectResult(r);
        }
    }
}