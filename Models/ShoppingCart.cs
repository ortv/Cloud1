using Cloud1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


namespace Cloud1.Models
{
    public class ShoppingCart
    {
        private readonly Cloud1Context _cloud1Context;
        private ShoppingCart(Cloud1Context cloud1Context)
        {
            _cloud1Context = cloud1Context;
            shoppingCartItems = new List<CartItem>();

        }
        public string ShoppingCartId { get; set;}
        public List<CartItem> shoppingCartItems { get; set;}
        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session=services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context=services.GetService<Cloud1Context>();
            string cartId = session.GetString("CartId")??Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);
            return new ShoppingCart(context) { ShoppingCartId = cartId };

        }
        //public void AddToCart(IceCream1 iceCream1,int amount)
        //{
        //    var shoppingCartItem = _cloud1Context.CartItem.SingleOrDefault(s => s.Cream1.Id == iceCream1.Id && s.Id == int.Parse(ShoppingCartId));
        //    if (shoppingCartItem == null)//if item doesnt exist in cart-creates it!
        //    {
        //        var item = new CartItem
        //        {
        //            idShopCart = int.Parse(ShoppingCartId),
        //            Cream1 = iceCream1,
        //            Amount = 1
        //        };
        //        _cloud1Context.shoppingCartItems.Add(item);
        //    }

        //}
    }
   
}
