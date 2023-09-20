using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud1.Models
{
    public class Order
    {
        public int Id { get; set; }

        // Foreign key for the User relationship
        public int UserId { get; set; }

        // Navigation property to User
        [ForeignKey("UserId")]

        public DateTime? OrderDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
