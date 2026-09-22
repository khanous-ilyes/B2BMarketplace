using BaseLibrary.Entities;
using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;

        public CategoriesController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        // GET: api/categories
        // Accessible à tous (clients non connectés inclus)
        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> GetCategories([FromQuery] bool includeDomain = false)
        {
            var result = await _repository.GetCategories(includeDomain);
            return Ok(result);
        }

        // GET: api/categories/domains
        // Accessible à tous
        [HttpGet("domains")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDomains()
        {
            var result = await _repository.GetDomains();
            return Ok(result);
        }

        // POST: api/categories
        // Admin seulement
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
                DomainId = dto.DomainId
            };

            var result = await _repository.AddCategory(category);
            if (!result.Flag) return BadRequest(result);
            return Ok(result);
        }

        // PUT: api/categories
        // Admin seulement
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            var result = await _repository.UpdateCategory(category);
            if (!result.Flag) return BadRequest(result);
            return Ok(result);
        }

        // DELETE: api/categories/{id}
        // Admin seulement
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _repository.DeleteCategory(id);
            if (!result.Flag) return BadRequest(result);
            return Ok(result);
        }

        // POST: api/categories/domains
        // Admin seulement
        [HttpPost("domains")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDomain([FromBody] CreateDomainDto dto)
        {
            var domain = new Domain
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty
            };

            var result = await _repository.AddDomain(domain);
            if (!result.Flag) return BadRequest(result);
            return Ok(result);
        }

        // DELETE: api/categories/domains/{id}
        // Admin seulement
        [HttpDelete("domains/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDomain(int id)
        {
            var result = await _repository.DeleteDomain(id);
            if (!result.Flag) return BadRequest(result);
            return Ok(result);
        }
    }
}
