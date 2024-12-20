using MonRestoAPI.Models;
using System.ComponentModel.DataAnnotations;

public class Payment
{
    public int Id { get; set; }

    [Required]
    [CreditCard]
    [StringLength(16, MinimumLength = 15, ErrorMessage = "Le numéro de carte doit contenir 15 ou 16 chiffres.")]
    public string CardNumber { get; set; }

    [Required]
    [StringLength(50)]
    public string CardHolderName { get; set; }

    [Required]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Format d'expiration invalide. (MM/AA)")]
    public string ExpirationDate { get; set; }

    [Required]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "Le CVV doit contenir exactement 3 chiffres.")]
    public string CVV { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être positif.")]
    public decimal Amount { get; set; } // Montant à payer

    public bool IsSuccessful { get; set; } // Indicateur de succès

    // Ajoutez la clé étrangère pour User
    [Required]
    public int UserId { get; set; } // Clé étrangère vers User

    // Propriété de navigation pour User (non requise dans ce cas)
    public User? User { get; set; }
}
