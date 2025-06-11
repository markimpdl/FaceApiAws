using FaceApi.DTOs;
using FaceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaceApi.Controllers
{
    [Authorize()]
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolsController : ControllerBase
    {
        private readonly ISchoolService _service;

        public SchoolsController(ISchoolService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSchoolDto dto)
        {
            var school = await _service.CreateAsync(dto.Name);
            return Ok(school);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Ok(await _service.GetByIdAsync(id));
    }
}

