using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonRestoAPI.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }  // Clé étrangère vers Order
        public Order Order { get; set; }

        [Required]
        public int ArticleId { get; set; }  // Clé étrangère vers Article
        public Article Article { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }  // Calculé à partir du prix de l'article
    }
}
