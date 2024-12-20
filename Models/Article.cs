using MonRestoAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Article
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }  // clé étrangère

    [ForeignKey("CategoryId")]  // Spécifie que CategoryId est la clé étrangère pour la relation
    public Category Category { get; set; }  // relation avec le modèle Category
}
