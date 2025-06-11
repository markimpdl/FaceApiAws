using Amazon.S3;
using Amazon.S3.Transfer;

namespace FaceApi.Services
{
    public class S3StorageService : IS3StorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucket;

        public S3StorageService(IAmazonS3 s3Client, IConfiguration config)
        {
            _s3Client = s3Client;
            _bucket = config["S3:Bucket"];
        }

        public async Task<string> UploadAsync(IFormFile file, string fileName)
        {
            using var stream = file.OpenReadStream();
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = fileName,
                BucketName = _bucket,
                ContentType = file.ContentType
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            return $"https://{_bucket}.s3.amazonaws.com/{fileName}";
        }
    }
}
