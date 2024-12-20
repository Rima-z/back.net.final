using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonRestoAPI.Data;
using MonRestoAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonRestoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArticlesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles(int? categoryId)
        {
            var articlesQuery = _context.Articles.Include(a => a.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                articlesQuery = articlesQuery.Where(a => a.CategoryId == categoryId.Value);
            }

            return await articlesQuery.ToListAsync();
        }


        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles.Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        // POST: api/Articles
        [HttpPost]
        public IActionResult CreateArticle([FromBody] Article article)
        {
            if (ModelState.IsValid)
            {
                // Vérifiez si la catégorie existe avant de l'associer à l'article
                var category = _context.Categories.Find(article.CategoryId);
                if (category == null)
                {
                    return BadRequest("Category not found.");
                }

                // Associer la catégorie à l'article sans essayer de modifier l'ID de la catégorie
                article.Category = category;

                _context.Articles.Add(article);
                _context.SaveChanges();
                return CreatedAtAction("GetArticle", new { id = article.Id }, article);
            }

            return BadRequest(ModelState);
        }






        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, Article article)
        {
            if (id != article.Id)
            {
                return BadRequest();
            }

            _context.Entry(article).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
