using System.ComponentModel.DataAnnotations;

namespace OrderService
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}