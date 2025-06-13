using Amazon.S3.Model;
using FaceApi.Data;
using FaceApi.DTOs;
using FaceApi.Models;
using FaceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FaceApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IS3StorageService _s3StorageService;
        private readonly IRekognitionService _rekognitionService;
        private readonly IConfiguration _config;
        private readonly ApiDbContext _db;

        public UsersController(IUserService userService,
            IS3StorageService s3StorageService,
            IRekognitionService rekognitionService,
            IConfiguration config,
        ApiDbContext db)
        {
            _userService = userService;
            _s3StorageService = s3StorageService;
            _rekognitionService = rekognitionService;
            _config = config;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUserDto dto)
        {
            if (dto.Photo == null)
                return BadRequest("Invalid Photo");
            if (dto.SchoolIds == null || dto.SchoolIds.Count == 0)
                return BadRequest("Invalid schools");

            // Verifica se todas as escolas existem
            var existingSchoolIds = await _db.Schools
                .Where(s => dto.SchoolIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            var missingSchools = dto.SchoolIds.Except(existingSchoolIds).ToList();
            if (missingSchools.Any())
                return BadRequest($"Invalid School IDs: {string.Join(", ", missingSchools)}");

            string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Photo.FileName)}";
            string photoUrl = await _s3StorageService.UploadAsync(dto.Photo, uniqueFileName);
            string bucket = _config["S3:Bucket"];
            var userSchools = new List<UserSchool>();

            foreach (var schoolId in dto.SchoolIds)
            {
                string collectionId = $"escola_{schoolId}";
                await _rekognitionService.CreateCollectionIfNotExistsAsync(collectionId);

                string faceId = await _rekognitionService.IndexFaceAsync(collectionId, bucket, uniqueFileName);
                userSchools.Add(new UserSchool { SchoolId = schoolId, AwsFaceId = faceId });
            }

            var user = new User
            {
                Name = dto.Name,
                BasePhotoUrl = photoUrl,
                UserSchools = userSchools,
                UserType = dto.UserType
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new UserDto { Id = user.Id, Name = user.Name, BasePhotoUrl = user.BasePhotoUrl, UserType=user.UserType });

        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _userService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Ok(await _userService.GetByIdAsync(id));
    }
}
