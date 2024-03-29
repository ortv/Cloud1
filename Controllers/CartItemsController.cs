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
using Humanizer;
//using Cloud1.Services;
//using GateWay.Models;
using Newtonsoft.Json;
using Microsoft.DotNet.MSIdentity.Shared;
using Cloud1.Services;
//using static GateWay.Models.hebcal;

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
            //saving defult values!
            order.Address = string.Empty;
            order.City = string.Empty;
            order.Email = string.Empty;
            order.Name = string.Empty;
            //order.DeliveryDate = DateTime.Now.AddMinutes(45);
            _context.Order.Add(order);
            await _context.SaveChangesAsync();
            OrderDetails details= new OrderDetails();
            details.cartItemsList = cartItemss.ToList();
            details.hebcalResponse = await HebcalService();
            details.order = order;

            WeatherResponse weather = new WeatherResponse();
            weather.City = string.Empty;
            weather.Humidity = 3;
            weather.FeelsLike = 2;
            details.weatherResponse=weather;
			_context.OrderDetails.Add(details);
			await _context.SaveChangesAsync();
			return RedirectToAction("Checkout", "Orders", order);
        }
		public async Task<HebcalResponse> HebcalService()
		{
			var apiService = new ApiService("acc_3d60a751e375dec");
			var hebcalApiResponse = await apiService.GetApiResponseAsync<HebcalResponse>("http://gatewayService.somee.com/api/Hebcal");
            //return (hebcalApiResponse.Day, hebcalApiResponse.IsHoliday);
            return hebcalApiResponse;
		}

        //add &&  remove from cart
        public async Task<IActionResult> AddToCart(int id, int amount)
        {
            ShoppingCartId = GetCartId();

            // Check if the user has already added 5 items to the cart
            var cartItemCount = _context.CartItem.Count(c => c.CartId == ShoppingCartId);

            if (cartItemCount >= 5)
            {
                // If the user has reached the limit, return a JSON response
                return Json(new { success = false, message = "You can only add up to 5 products to the cart." });
            }

            var cartItem = await _context.CartItem.SingleOrDefaultAsync(
                c => c.CartId == ShoppingCartId && c.Cream1.Id == id);

            if (cartItem == null)
            {
                // Create a new cart item if it doesn't exist.
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    CartId = ShoppingCartId,
                    Cream1 = GetIceCreamById(id),
                    Quantity = 1,
                    DateCreated = DateTime.Now,
                    Price = amount * GetIceCreamById(id).Price
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

            try
            {
                await _context.SaveChangesAsync();
                var cartCount = _context.CartItem.Count(c => c.CartId == ShoppingCartId);

                // Return a JSON response with success and cart count
                return Json(new { success = true, message = "Item added to the cart successfully.", cartCount });

                // Return a JSON response indicating success
                //return Json(new { success = true, message = "Item added to the cart successfully." });
            }
            catch (Exception ex)
            {
                // Handle exceptions if needed

                // Return a JSON response indicating failure
                return Json(new { success = false, message = "Failed to add item to the cart." });
            }
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
        public IActionResult GetCartCount()
        {
            var cartCount = _context.CartItem.Count(c => c.CartId == GetCartId());
            return Json(new { cartCount });
        }


    }
}

