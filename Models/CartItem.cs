using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud1.Models
{
    public class CartItem
    {
        public int IdItemCart { get; set; } // product id
        public int idShopCart { get; set; }
        public IceCream1 Cream1 { get; set; }

        public int Amount { get; set; } // amount of products

        //public double Price { get; set; } // price for each product
        public double TotalPrice { get; set; } // total price for all amount of this product
        
        // public string? ImageUrl { get; set; } // option of no image
    }
}
