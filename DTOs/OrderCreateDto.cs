namespace MonRestoAPI.DTOs
{
    public class OrderCreateDto
    {
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public List<OrderItemCreateDto> OrderItems { get; set; }
    }
}
