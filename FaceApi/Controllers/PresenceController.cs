using FaceApi.Data;
using FaceApi.DTOs;
using FaceApi.Models;
using FaceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FaceApi.Controllers
{
    [AllowAnonymous] 
    [ApiController]
    [Route("api/[controller]")]
    public class PresenceController : ControllerBase
    {
        private readonly IPresenceService _presenceService;
        private readonly IRekognitionService _rekognitionService;
        private readonly ApiDbContext _db;

        public PresenceController(
            IPresenceService presenceService,
            IRekognitionService rekognitionService,
            ApiDbContext db)
        {
            _presenceService = presenceService;
            _rekognitionService = rekognitionService;
            _db = db;
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> Checkin([FromForm] PresenceCheckinDto dto)
        {
            string collectionId = $"escola_{dto.SchoolId}";
            string faceId;
            using (var stream = dto.Photo.OpenReadStream())
            {
                faceId = await _rekognitionService.SearchFaceByImageAsync(collectionId, stream);
            }

            if (string.IsNullOrEmpty(faceId))
                return NotFound("Rosto não reconhecido.");

            var userSchool = await _db.UserSchools
                .Include(us => us.User)
                .FirstOrDefaultAsync(us => us.SchoolId == dto.SchoolId && us.AwsFaceId == faceId);

            if (userSchool == null)
                return NotFound("Usuário não encontrado nesta escola.");
            // 4. Registrar presença no banco (service)
            var record = await _presenceService.RegisterAsync(userSchool.UserId, dto.SchoolId);

            return Ok(new
            {
                Mensagem = "Presença registrada com sucesso!",
                Professor = userSchool.User.Name,
                Escola = userSchool.SchoolId,
                DataHora = record.DateTime
            });
        }

        [Authorize]
        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] int? userId,
            [FromQuery] int? schoolId)
        {
            var result = await _presenceService.FilterAsync(start, end, userId, schoolId);
            return Ok(result);
        }
    }
}