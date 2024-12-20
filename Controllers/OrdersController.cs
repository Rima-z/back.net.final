using Microsoft.AspNetCore.Mvc;
using MonRestoAPI.Data;
using MonRestoAPI.Models;
using Microsoft.EntityFrameworkCore;
using MonRestoAPI.DTOs;

namespace MonRestoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Article)
                .ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Article)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return order;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderCreateDto orderCreateDto)
        {
            // Créer une nouvelle instance de Order à partir du DTO
            var order = new Order
            {
                CustomerName = orderCreateDto.CustomerName,
                OrderDate = DateTime.Now,
                OrderStatus = orderCreateDto.OrderStatus,
                OrderItems = new List<OrderItem>()
            };

            decimal totalPrice = 0;

            // Mapper OrderItems
            foreach (var itemDto in orderCreateDto.OrderItems)
            {

                var article = await _context.Articles.FindAsync(itemDto.ArticleId);
                if (article == null)
                {
                    return BadRequest($"L'article avec l'ID {itemDto.ArticleId} n'existe pas.");
                }
                var orderItem = new OrderItem
                {
                    ArticleId = itemDto.ArticleId,
                    Quantity = itemDto.Quantity,
                    Price = article.Price
                };

                // Calculer le prix total de la commande
                totalPrice += orderItem.Price * orderItem.Quantity;

                order.OrderItems.Add(orderItem);
            }

            // Calculer le total de la commande
            order.TotalPrice = totalPrice;

            // Ajouter la commande à la base de données
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Retourner la réponse avec l'objet Order complet
            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }


        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            if (id != order.Id)
                return BadRequest();

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(o => o.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}