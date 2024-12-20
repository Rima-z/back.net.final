using Microsoft.AspNetCore.Mvc;
using MonRestoAPI.Models;
using MonRestoAPI.Repositories;

namespace MonRestoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _repository;

        public CategoriesController(IRepository<Category> repository)
        {
            _repository = repository;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _repository.GetAllAsync();
            return Ok(categories);
        }

        // GET: api/Categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _repository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            return Ok(category);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            await _repository.AddAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        // PUT: api/Categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest("L'identifiant dans l'URL ne correspond pas à celui de la catégorie.");
            }

            // Vérifiez si la catégorie existe dans la base de données
            var existingCategory = await _repository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound($"La catégorie avec l'identifiant {id} n'existe pas.");
            }

            // Mettez à jour uniquement les propriétés nécessaires
            existingCategory.Name = category.Name;

            // Mettez à jour l'entité existante
            await _repository.UpdateAsync(existingCategory);

            return NoContent();
        }


        // DELETE: api/Categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            await _repository.DeleteAsync(id); // Passez l'ID ici
            return NoContent();
        }

    }
}
