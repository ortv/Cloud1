namespace Cloud1.Models
{
    public class CartView
    {
        public List<CartItem> CartItems { get; set; }
        public List<IceCream1> iceCreams { get; set; }
        public double Total()//total for whole cart
        {
            double total = 0;
            foreach (var item in CartItems) { total += item.Price; }
            return total;
        }
    }
}
