using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud1.Models
{
    public class Order
    {
            public int Id { get; set; }
            public DateTime? OrderDate { get; set; }
            public DateTime? ShipDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public double TotalPrice { get; set; }

    }
}
