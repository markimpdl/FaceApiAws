using FaceApi.Data;
using FaceApi.DTOs;
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
        private readonly IAzureFaceService _faceService;
        private readonly ApiDbContext _db;

        public PresenceController(
            IPresenceService presenceService,
            IAzureFaceService faceService,
            ApiDbContext db)
        {
            _presenceService = presenceService;
            _faceService = faceService;
            _db = db;
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> Checkin([FromForm] PresenceCheckinDto dto)
        {
            // 1. Detectar o rosto na foto do check-in
            using var stream = dto.Photo.OpenReadStream();
            string faceId;
            try
            {
                faceId = await _faceService.DetectFaceAsync(stream);
            }
            catch
            {
                return BadRequest("Nenhum rosto detectado na imagem.");
            }

            // 2. Identificar o professor no grupo da escola
            string groupId = $"escola_{dto.SchoolId}";
            string personId = await _faceService.IdentifyAsync(groupId, faceId);

            if (personId == null)
                return NotFound("Rosto não reconhecido.");

            // 3. Encontrar o professor vinculado a esse personId
            var userSchool = await _db.UserSchools
                .Include(us => us.User)
                .FirstOrDefaultAsync(us => us.SchoolId == dto.SchoolId && us.AzurePersonId == personId);

            if (userSchool == null)
                return NotFound("Professor não encontrado nesta escola.");

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