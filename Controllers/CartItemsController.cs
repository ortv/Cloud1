﻿using System;
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

        public async Task<IActionResult> Index()
        {
            var cartItems = GetCartItems();
            var iceList = new List<IceCream>();

            foreach (var item in cartItems)
            {
                var ice = GetIceCreamById(Convert.ToInt32(item.ItemId));
                iceList.Add(ice);

            }

            var model = new CartView
            {
                CartItems = cartItems,
                iceCreams = iceList
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
            var cartItemss = GetCartItems();
            var iceCreamss = new List<IceCream>();

            foreach (var item in cartItemss)
            {
                var ice = GetIceCreamById(Convert.ToInt32(item.ItemId));
                iceCreamss.Add(ice);
            }

            var cart = new CartView
            {
                CartItems = cartItemss,
                iceCreams = iceCreamss
            };
            //Order order = new Order() { Products = cart.CartItems, Total= cart.Total() };
            Order order = new Order() { OrderDate=DateTime.Now, TotalPrice = cart.Total()  };
            string orderJson = JsonSerializer.Serialize(order);

            // Pass it as a route value
            return RedirectToAction("Checkout", "Orders", new { order = orderJson });

            //return RedirectToAction("Checkout", "Orders", order);
            // To open a view from a different controller
            //return View("~/Views/Orders/Checkout.cshtml", order);
        }

        //add &&  remove from cart
        public async Task AddToCart(int id, int amount)//amount-how many items to add
        {
            ShoppingCartId = GetCartId();

            var cartItem = await _context.CartItem.SingleOrDefaultAsync(
                c => c.CartId == ShoppingCartId && Convert.ToInt32(c.ItemId) == id);

            if (cartItem == null)
            {
                // Create a new cart item if it doesn't exist.

                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    CartId = ShoppingCartId,
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
                cartItem.Price += amount * GetIceCreamById(id).Price;
                cartItem.Price.ToString("F3");
            }
            try { await _context.SaveChangesAsync(); }
            catch (Exception ex) { }

        }

        public async Task<IActionResult> RemoveFromCart(string id)
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

            return _context.CartItem.Where(
                c => c.CartId == ShoppingCartId).ToList();
        }
        public IceCream GetIceCreamById(int id)
        {
            return _context.IceCream.SingleOrDefault(p => p.Id == id);
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
    }
}
