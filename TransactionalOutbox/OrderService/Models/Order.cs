using System.ComponentModel.DataAnnotations;

namespace OrderService
{
    public class Order
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}