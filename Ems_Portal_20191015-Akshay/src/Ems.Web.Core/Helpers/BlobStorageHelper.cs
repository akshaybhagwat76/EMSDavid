using Abp.IO.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ems.Web.Helpers
{
    public class BlobStorageHelper
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly CloudBlobContainer _container;
        private readonly Uri _blobServiceEndpoint = new Uri("https://aelfilesandimages.blob.core.windows.net/");
        private readonly string _containerName = "aelcontainer";

        public BlobStorageHelper()
        {
            var credentials = new StorageCredentials("aelfilesandimages", "tsQkUeS4/EhdpnDL81RDfPLJLslYAoO/ObiB+9d8XItDB+tcWlHaUXPe0fLkKmLYxHIh06qUgeXxpWCKQf87fA==");
            _cloudBlobClient = new CloudBlobClient(_blobServiceEndpoint, credentials);
            _container = _cloudBlobClient.GetContainerReference(_containerName);
        }

        private static string GenerateFileName()
        {
            return $"{DateTime.UtcNow.ToString("yyyyMMddhhmmssffff")}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        public Uri GetResourceUriWithSas(string resourcePath)
        {
            if (!string.IsNullOrWhiteSpace(resourcePath))
            {
                var sasPolicy = new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessStartTime = DateTime.Now.AddMinutes(-1),
                    SharedAccessExpiryTime = DateTime.Now.AddMinutes(30)
                };

                CloudBlockBlob blob = _container.GetBlockBlobReference(resourcePath);
                string sasToken = blob.GetSharedAccessSignature(sasPolicy);

                return new Uri($"{_blobServiceEndpoint}{_containerName}/{resourcePath}{sasToken}");
            }

            return null;
        }

        public async Task<string> SaveAttachment(IFormFile aFile, string subDirectory)
        {
            string id = $"{subDirectory}/{GenerateFileName()}{Path.GetExtension(aFile.FileName)}";
            CloudBlockBlob blob = _container.GetBlockBlobReference(id);
            blob.Properties.ContentType = aFile.ContentType;

            byte[] fileBytes;
            using (var stream = aFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }
            Stream strm = new MemoryStream(fileBytes);

            await blob.UploadFromStreamAsync(strm);

            Array.Clear(fileBytes, 0, fileBytes.Length);
            strm.Dispose();

            return id;
        }
    }
}
