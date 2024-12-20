using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonRestoAPI.Data;
using MonRestoAPI.Models;

namespace MonRestoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Injectez le contexte de la base de données
        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult ProcessPayment([FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifier si l'utilisateur existe
            var user = _context.Users.Find(payment.UserId); // Assume _context is your DbContext
            if (user == null)
            {
                return BadRequest("Utilisateur introuvable.");
            }

            // Lier l'utilisateur au paiement
            payment.User = user;

            // Simuler le succès du paiement
            payment.IsSuccessful = true; // Par défaut, toujours réussi. Vous pouvez intégrer un vrai service de paiement ici.

            // Enregistrer le paiement dans la base de données
            _context.Payments.Add(payment);
            _context.SaveChanges();

            if (payment.IsSuccessful)
            {
                return Ok("Paiement validé.");
            }

            return BadRequest("Échec du paiement.");
        }
    }
}
