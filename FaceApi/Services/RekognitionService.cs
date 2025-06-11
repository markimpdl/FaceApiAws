using Amazon.Rekognition;
using Amazon.Rekognition.Model;

namespace FaceApi.Services
{
    public class RekognitionService : IRekognitionService
    {
        private readonly IAmazonRekognition _rekognition;

        public RekognitionService(IAmazonRekognition rekognition)
        {
            _rekognition = rekognition;
        }

        public async Task CreateCollectionIfNotExistsAsync(string collectionId)
        {
            var listResponse = await _rekognition.ListCollectionsAsync(new ListCollectionsRequest());

            if (!listResponse.CollectionIds.Contains(collectionId))
                await _rekognition.CreateCollectionAsync(new CreateCollectionRequest { CollectionId = collectionId });
        }

        public async Task<string> IndexFaceAsync(string collectionId, string bucket, string key)
        {
            var request = new IndexFacesRequest
            {
                CollectionId = collectionId,
                Image = new Image { S3Object = new S3Object { Bucket = bucket, Name = key } },
                DetectionAttributes = new List<string> { "DEFAULT" }
            };
            var response = await _rekognition.IndexFacesAsync(request);
            return response.FaceRecords.FirstOrDefault()?.Face?.FaceId;
        }

        public async Task<string> SearchFaceByImageAsync(string collectionId, Stream imageStream)
        {
            var request = new SearchFacesByImageRequest
            {
                CollectionId = collectionId,
                Image = new Image { Bytes = new MemoryStream(ReadFully(imageStream)) },
                MaxFaces = 1,
                FaceMatchThreshold = 90
            };
            var response = await _rekognition.SearchFacesByImageAsync(request);
            return response.FaceMatches.FirstOrDefault()?.Face.FaceId;
        }

        private static byte[] ReadFully(Stream input)
        {
            using var ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
