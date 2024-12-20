using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MonRestoAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }  // Calculé automatiquement

        [Required]
        public string OrderStatus { get; set; } // Ex : "En cours", "Livré", "Annulé"

        // Relation avec OrderItem
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
