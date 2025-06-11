using System.Net.Http.Headers;
using System.Text.Json;

namespace FaceApi.Services
{
    public class AzureFaceService : IAzureFaceService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public AzureFaceService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        private string Endpoint => _config["AzureFaceApi:Endpoint"];
        private string Key => _config["AzureFaceApi:Key"];

        private void AddHeaders()
        {
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Key);
        }

        public async Task CreatePersonGroupAsync(string groupId, string name)
        {
            AddHeaders();
            var url = $"{Endpoint}/face/v1.0/persongroups/{groupId}";
            var data = new { name };

            var res = await _http.PutAsJsonAsync(url, data);

            // Se for 409, o grupo já existe: não é um erro fatal, só continue
            if (res.StatusCode == System.Net.HttpStatusCode.Conflict)
                return;

            // Se não for sucesso, lance a exceção
            if (!res.IsSuccessStatusCode)
            {
                var err = await res.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao criar PersonGroup: {res.StatusCode} - {err}");
            }
        }


        // Criar pessoa (person) no grupo
        public async Task<string> CreatePersonAsync(string groupId, string personName)
        {
            AddHeaders();
            var url = $"{Endpoint}/face/v1.0/persongroups/{groupId}/persons";
            var data = new { name = personName };
            var res = await _http.PostAsJsonAsync(url, data);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("personId").GetString();
        }

        // Adicionar face base ao Person (URL da foto base)
        public async Task AddFaceToPersonAsync(string groupId, string personId, string photoUrl)
        {
            AddHeaders();
            var url = $"{Endpoint}/face/v1.0/persongroups/{groupId}/persons/{personId}/persistedFaces";
            var data = new { url = photoUrl };
            var res = await _http.PostAsJsonAsync(url, data);
            res.EnsureSuccessStatusCode();
        }

        // Treinar grupo
        public async Task TrainPersonGroupAsync(string groupId)
        {
            AddHeaders();
            var url = $"{Endpoint}/face/v1.0/persongroups/{groupId}/train";
            var res = await _http.PostAsync(url, null);
            res.EnsureSuccessStatusCode();
        }

        // Detectar rosto na foto recebida (no check-in)
        public async Task<string> DetectFaceAsync(Stream photoStream)
        {
            AddHeaders();
            var url = $"{Endpoint}/face/v1.0/detect?returnFaceId=true";
            var content = new StreamContent(photoStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var res = await _http.PostAsync(url, content);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var faceId = doc.RootElement[0].GetProperty("faceId").GetString();
            return faceId;
        }

        // Identificar pessoa pelo faceId, no grupo
        public async Task<string> IdentifyAsync(string groupId, string faceId)
        {
            AddHeaders();
            var url = $"{Endpoint}/face/v1.0/identify";
            var payload = new
            {
                personGroupId = groupId,
                faceIds = new[] { faceId },
                maxNumOfCandidatesReturned = 1,
                confidenceThreshold = 0.7
            };
            var res = await _http.PostAsJsonAsync(url, payload);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.GetArrayLength() == 0)
                return null;
            var candidates = doc.RootElement[0].GetProperty("candidates");
            if (candidates.GetArrayLength() == 0)
                return null;
            return candidates[0].GetProperty("personId").GetString();
        }
    }
}
