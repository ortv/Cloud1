using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cloud1.Data;
using Cloud1.Models;
using System.Text;
using System.Text.Json;
using Humanizer;

namespace Cloud1.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly Cloud1Context _context;
        public static string ShoppingCartId { get; set; } = "";
        public const string CartSessionKey = "CartId";

        public CartItemsController(Cloud1Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cartItems = _context.CartItem
                .Include(c => c.Cream1) // Eager loading to include related IceCream1
                .Where(c => c.CartId == GetCartId())
                .ToList();
            var iceCreams = cartItems.Select(c => c.Cream1).ToList(); // Extract associated IceCream1 entities

            var model = new CartView
            {
                CartItems = cartItems,
                iceCreams=iceCreams
            };

            return View(model);
        }

        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.CartItem == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,CartId,Quantity,DateCreated,Price,OrderId")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.CartItem == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ItemId,CartId,Quantity,DateCreated,Price,OrderId")] CartItem cartItem)
        {
            if (id != cartItem.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.ItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cartItem);
        }

        // GET: CartItems/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.CartItem == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.CartItem == null)
            {
                return Problem("Entity set 'Cloud1Context.CartItem'  is null.");
            }
            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartItemExists(string id)
        {
          return (_context.CartItem?.Any(e => e.ItemId == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Checkout()
        {
            //var cartItem = await _context.CartItem
            //.Include(c => c.Cream1)
            //.SingleOrDefaultAsync(c => c.CartId == ShoppingCartId && c.ItemId == id);
            var cartItemss = GetCartItems();

            var iceCreamss = new List<IceCream1>();

            foreach (var item in cartItemss)
            {
                var ice = GetIceCreamById(item.Cream1.Id);
                iceCreamss.Add(ice);
            }

            var cart = new CartView
            {
                CartItems = cartItemss,
                iceCreams = iceCreamss
            };
            //Order order = new Order() { Products = cart.CartItems, Total= cart.Total() };
            Order order = new Order() { OrderDate = DateTime.Now, TotalPrice = cart.Total(), };
            ////Add the order to the context(in -memory representation)
            //_context.Order.Add(order);
            //// Save changes to the database
            //await _context.SaveChangesAsync();
            //string orderJson = JsonSerializer.Serialize(order);
            //return View(order);
            // Pass it as a route value
            return RedirectToAction("Checkout", "Orders", order);
        }

        //add &&  remove from cart
        public async Task AddToCart(int id, int amount)//amount-how many items to add
        {
            ShoppingCartId = GetCartId();

            var cartItem = await _context.CartItem.SingleOrDefaultAsync(
                c => c.CartId == ShoppingCartId && c.Cream1.Id == id);

            if (cartItem == null)
            {
                // Create a new cart item if it doesn't exist.

                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    CartId = ShoppingCartId,
                    Cream1= GetIceCreamById(id),
                    Quantity = 1,
                    DateCreated = DateTime.Now,
                    Price = amount * GetIceCreamById(id).Price//price for one multiple the amount
                    // OrderId=6
                };

                _context.CartItem.Add(cartItem);
            }
            else
            {
                // If the item exists in the cart, increment the quantity.
                cartItem.Quantity += amount;
                cartItem.Price = cartItem.Quantity * GetIceCreamById(id).Price;
                cartItem.Price.ToString("F3");
            }
            try { await _context.SaveChangesAsync(); }
            catch (Exception ex) { }

        }

        public async Task<IActionResult> RemoveFromCart(string id)
        {
            ShoppingCartId = GetCartId();

            var cartItem = await _context.CartItem
            .Include(c => c.Cream1)
            .SingleOrDefaultAsync(c => c.CartId == ShoppingCartId && c.ItemId == id);


            //var cartItem = await _context.CartItem.SingleOrDefaultAsync(
            //  c => c.CartId == ShoppingCartId && c.ItemId == id);

            if (cartItem != null)
            {
                if(cartItem.Quantity>1)
                {
                    cartItem.Quantity -= 1;
                    cartItem.Price -= cartItem.Cream1.Price;
                }
                else
                {
                _context.CartItem.Remove(cartItem);

                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> RemoveFromCart2(string id)
        {
            ShoppingCartId = GetCartId();

            var cartItem = await _context.CartItem.SingleOrDefaultAsync(
              c => c.CartId == ShoppingCartId && c.ItemId == id);

            if (cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }



        //help functions:
        public string GetCartId()
        {
            if (HttpContext.Session.Get(CartSessionKey) == null || TempData["OrderCompleted"] != null)
            {
                if (!string.IsNullOrWhiteSpace(User.Identity.Name))
                {
                    var bytes1 = Encoding.UTF8.GetBytes(User.Identity.Name);
                    HttpContext.Session.Set(CartSessionKey, bytes1);
                }
                else
                {
                    // Generate a new random GUID using System.Guid class.
                    Guid tempCartId = Guid.NewGuid();
                    var bytes1 = Encoding.UTF8.GetBytes(tempCartId.ToString());
                    HttpContext.Session.Set(CartSessionKey, bytes1);
                }

                // Remove the TempData flag to indicate that the order is not completed.
                TempData.Remove("OrderCompleted");
            }

            var bytes = HttpContext.Session.Get(CartSessionKey);
            return Encoding.UTF8.GetString(bytes);
        }

        public List<CartItem> GetCartItems()
        {
            ShoppingCartId = GetCartId();

            var lst= _context.CartItem.Include(c => c.Cream1).Where(
                c => c.CartId == ShoppingCartId).ToList();
            return lst;
        }
        public IceCream1 GetIceCreamById(int id)
        {
            return _context.IceCream1.SingleOrDefault(p => p.Id == id);
        }
        public string GetFlavourNameById(int id)
        {
            return GetIceCreamById(id).IceName;
        }
        // Dispose of the database context properly.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        public JsonResult GetCartItemCount()
        {
            var cartItemsCount = _context.CartItem
                .Count(c => c.CartId == GetCartId());

            return Json(cartItemsCount);
        }

    }
}

