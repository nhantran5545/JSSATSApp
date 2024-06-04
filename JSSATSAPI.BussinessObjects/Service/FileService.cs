using Azure.Storage.Blobs;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class FileService : IFileService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "jssats";

        public FileService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> Upload(FileRequest fileRequest)
        {
            var containerInstance = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobName = Path.GetFileName(fileRequest.imageFile.FileName);

            // Kiểm tra xem blob đã tồn tại trong container chưa
            var blobClient = containerInstance.GetBlobClient(blobName);
            if (await blobClient.ExistsAsync())
            {
                blobName = GetUniqueBlobName(containerInstance, blobName);
            }

            var blobInstance = containerInstance.GetBlobClient(blobName);
            await blobInstance.UploadAsync(fileRequest.imageFile.OpenReadStream());

            return blobInstance.Uri.ToString();
        }

        public async Task<Stream> Get(string name)
        {
            var containerInstance = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobInstance = containerInstance.GetBlobClient(name);
            var downloadContent = await blobInstance.DownloadAsync();
            return downloadContent.Value.Content;
        }

        public bool IsImageFile(string fileName)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            string extension = Path.GetExtension(fileName).ToLower();
            return allowedExtensions.Contains(extension);
        }

        private string GetUniqueBlobName(BlobContainerClient containerClient, string blobName)
        {
            // Tạo một tên mới cho blob
            string uniqueBlobName = blobName;
            int counter = 1;

            while (containerClient.GetBlobClient(uniqueBlobName).Exists())
            {
                string extension = Path.GetExtension(blobName);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(blobName);
                uniqueBlobName = $"{fileNameWithoutExtension}_{counter}{extension}";
                counter++;
            }

            return uniqueBlobName;
        }
    }
}
