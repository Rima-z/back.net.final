using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonRestoAPI.Models;
using MonRestoAPI.Data;  // Ajoutez cette ligne pour importer votre DbContext
using System.Threading.Tasks;

namespace MonRestoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;  // Utilisez ApplicationDbContext ici

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Contact
        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Contacts.Add(contact);  // Accédez à la table Contacts
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contact saved successfully!" });
        }
    }
}
