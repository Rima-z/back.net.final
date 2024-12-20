namespace MonRestoAPI.Models
{
    public class User
    {
        public int Id { get; set; } // Clé primaire
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // Stockez les mots de passe de manière sécurisée
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //public Profile Profile { get; set; }

    }
}