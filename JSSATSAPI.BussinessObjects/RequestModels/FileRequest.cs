using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels
{
    public class FileRequest
    {
        public IFormFile imageFile { get; set; }
    }
}
