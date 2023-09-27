using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud1.Models
{
    public class CartItem
    {
        public int Id { get; set; } // cart item id
        public int ShoppingCartId { get; set; } // Foreign key to ShoppingCart

        // Navigation property for ShoppingCart
        public ShoppingCart ShoppingCart { get; set; }
        public IceCream1 Cream1 { get; set; }
        public System.DateTime DateCreated { get; set; }

        public int Amount { get; set; } // amount of products

        //public double Price { get; set; } // price for each product
        public double TotalPrice { get; set; } // total price for all amount of this product
        
        // public string? ImageUrl { get; set; } // option of no image
    }
}
