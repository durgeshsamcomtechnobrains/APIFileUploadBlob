using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace ApiFileUpload
{
    public class FileService
    {
        //private readonly string _storageAccount = "StorageAccount";
        //private readonly string _key = "StorageAccountKey";
        private readonly BlobContainerClient _fileContainer;

        public FileService(IOptions<StorageAccountOptions> options)
        {
            var storageAccount = options.Value.AccountName;
            var key = options.Value.AccountKey;

            var credential = new StorageSharedKeyCredential(storageAccount, key);
            var blobUri = $"https://{storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _fileContainer = blobServiceClient.GetBlobContainerClient("files");

            //var credential = new StorageSharedKeyCredential (_storageAccount, _key);
            //var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            //var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            //_fileContainer = blobServiceClient.GetBlobContainerClient("files");
        }

        public async Task<List<BlobDto>> ListAsync()
        {
            List<BlobDto> files = new List<BlobDto>();
            await foreach (var file in _fileContainer.GetBlobsAsync())
            {
                string uri = _fileContainer.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                files.Add(new BlobDto
                {
                    Uri = fullUri,
                    Name = name,
                    ContentType = file.Properties.ContentType
                });
            }
            return files;
        }

        public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
        {
            BlobResponseDto response = new();
            BlobClient client = _fileContainer.GetBlobClient(blob.FileName);

            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

            return response;
        }

        public async Task<BlobDto?> DownloadAsync(string blobFilename)
        {
            BlobClient file = _fileContainer.GetBlobClient(blobFilename);

            if (await file.ExistsAsync())
            {
                var data = await file.OpenReadAsync();
                Stream blobContent = data;

                var content = await file.DownloadContentAsync();

                string name = blobFilename;
                string contentType = content.Value.Details.ContentType;

                return new BlobDto { Content = blobContent, Name = name, ContentType = contentType };
            }
            return null;
        }

        public async Task<BlobResponseDto> DeleteAsync (string blobFilename)
        {
            BlobClient file = _fileContainer.GetBlobClient(blobFilename);

            await file.DeleteAsync();

            return new BlobResponseDto { Error = false, Status = $"File: {blobFilename} has been successfully delete." };
        }
    }
}
