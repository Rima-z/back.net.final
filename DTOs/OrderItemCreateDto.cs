namespace MonRestoAPI.DTOs
{
    public class OrderItemCreateDto
    {
        public int ArticleId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
