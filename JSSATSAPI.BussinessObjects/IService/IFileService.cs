using JSSATSAPI.BussinessObjects.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IFileService
    {
        Task<string> Upload(FileRequest fileRequest);
        Task<Stream> Get(string name);
        bool IsImageFile(string fileName);
    }
}
