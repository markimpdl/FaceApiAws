using Amazon.Rekognition.Model;
using FaceApi.Data;
using FaceApi.DTOs;
using FaceApi.Enums;
using FaceApi.Models;
using FaceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;

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

        [HttpPost("pre-checkin")]
        public async Task<IActionResult> PreCheckin([FromForm] PreCheckinDto dto)
        {
            if (dto.SchoolId == null || dto.SchoolId == 0)
                return BadRequest("Invalid SchoolId");

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
            //var record = await _presenceService.RegisterAsync(userSchool.UserId, dto.SchoolId);

            return Ok(new 
            {
                UserId = userSchool.User.Id,
                RekognitionId = userSchool.AwsFaceId,
                Name = userSchool.User.Name,
                UserType = userSchool.User.UserType,
                BasePhotoUrl = userSchool.User.BasePhotoUrl
            });
        }


        [HttpPost("checkin")]
        public async Task<IActionResult> Checkin([FromForm] CheckinDto dto)
        {
           
            if (dto.UserId == null || dto.UserId == 0)
                return BadRequest("Invalid UserId");

            if (dto.SchoolId == null || dto.SchoolId == 0)
                return BadRequest("Invalid SchoolId");

            if (string.IsNullOrEmpty(dto.RekognitionId))
                return BadRequest("Invalid RekognitionId.");




            // 4. Registrar presença no banco (service)
            var record = await _presenceService.RegisterAsync(dto.UserId, dto.SchoolId, dto.RekognitionId, dto.GeoLocalization, dto.UserType);

            var uaeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(record.DateTime, uaeTimeZone);

            return Ok(new
            {
                message = "Attendance registered " + localDateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }


        [Authorize]
        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] int? userId,
            [FromQuery] int? schoolId,
            [FromQuery] UserType? userType
            )
        {
            var result = await _presenceService.FilterAsync(start, end, userId, schoolId, userType);
            return Ok(result);
        }
    }
}