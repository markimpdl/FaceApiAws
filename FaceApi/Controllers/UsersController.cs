using FaceApi.Data;
using FaceApi.DTOs;
using FaceApi.Models;
using FaceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaceApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IAzureFaceService _azureFaceService;
        private readonly ApiDbContext _db;

        public UsersController(IUserService userService, 
            IAzureBlobService azureBlobService,
            IAzureFaceService azureFaceService,
            ApiDbContext db)
        {
            _userService = userService;
            _azureBlobService = azureBlobService;
            _azureFaceService = azureFaceService;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUserDto dto)
        {
            if (dto.Photo == null || dto.Photo.Length == 0)
                return BadRequest("É necessário enviar uma foto.");

            // 1. Upload da foto base para o Blob
            string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Photo.FileName)}";
            string photoUrl = await _azureBlobService.UploadAsync(dto.Photo, uniqueFileName);

            // 2. Prepara lista de UserSchool já com AzurePersonId
            var userSchools = new List<UserSchool>();

            foreach (var schoolId in dto.SchoolIds)
            {
                string groupId = $"escola_{schoolId}";
                await _azureFaceService.CreatePersonGroupAsync(groupId, groupId);
                string personId = await _azureFaceService.CreatePersonAsync(groupId, dto.Name);
                await _azureFaceService.AddFaceToPersonAsync(groupId, personId, photoUrl);
                await _azureFaceService.TrainPersonGroupAsync(groupId);

                userSchools.Add(new UserSchool { SchoolId = schoolId, AzurePersonId = personId });
            }

            // 3. Monta User e salva tudo de uma vez só
            var user = new User
            {
                Name = dto.Name,
                BasePhotoUrl = photoUrl,
                UserSchools = userSchools
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(user);
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _userService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Ok(await _userService.GetByIdAsync(id));
    }
}
