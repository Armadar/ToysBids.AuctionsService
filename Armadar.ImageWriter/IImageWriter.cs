﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Armadar.ImageWriter
{
    public interface IImageWriter
    {
        Task<string> UploadImage(IFormFile file);
    }
}