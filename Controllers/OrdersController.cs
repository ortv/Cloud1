using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cloud1.Data;
using Cloud1.Models;
using Newtonsoft.Json;
using Cloud1.Services;
using Microsoft.VisualBasic;
using Cloud1.Migrations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Mail;
using System.Net;

//using Cloud1.Services;

namespace Cloud1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly Cloud1Context _context;

        public OrdersController(Cloud1Context context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return _context.Order != null ?
                        View(await _context.Order.ToListAsync()) :
                        Problem("Entity set 'Cloud1Context.Order'  is null.");
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            //TempData["TotalPrice"] = totalPrice;
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderDate,ShipDate,DeliveryDate,TotalPrice,Name,Address,City,Email")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,ShipDate,DeliveryDate,TotalPrice,Name,Address,City,Email")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var detailsForOrder=await _context.OrderDetails.Include(c => c.weatherResponse).
                Include(c => c.hebcalResponse).Include(c => c.order).Where(or=>or.order.Id== id).FirstOrDefaultAsync(); //takes out the details of this order
            if (detailsForOrder == null)
            {
                return NotFound(); // Or handle the case where the order is not found
            }
            return View(detailsForOrder);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'Cloud1Context.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> Checkout(Order order)
        {

            return View(order);
        }
        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode, decimal totalPrice)
        {
            // Implement coupon processing logic here and apply changes to totalPrice
            if (couponCode == "ICE10")
            {
                // Apply your specific coupon logic here
                totalPrice -= 10; // Adjust the price as per your coupon logic
            }

            // Return a response with the modified totalPrice
            return Json(new { message = "Coupon applied successfully", updatedTotalPrice = totalPrice });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Order updatedOrder)
        {
            //first,need to check if its a valid address
            if (ModelState.IsValid)
            {
                bool exist = await CheckAddress(updatedOrder.City, updatedOrder.Address);
                if (exist)//correct
                {
                    // Add the order to the context (in-memory representation)
                    //_context.Order.Add(updatedOrder);
                    //need to update the weather in the OrderDetails object
                    var order = _context.Order.OrderByDescending(e => e.Id).FirstOrDefault();
                    // order = updatedOrder;
                    //updeate db for order
                    order.Address = updatedOrder.Address;
                    order.OrderDate = updatedOrder.OrderDate;
                    order.TotalPrice = updatedOrder.TotalPrice;
                    order.City = updatedOrder.City;
                    order.Email = updatedOrder.Email;
                    order.Name = updatedOrder.Name; // Add this line
                    order.DeliveryDate = updatedOrder.DeliveryDate;
                    
                    

                    var details = _context.OrderDetails.OrderByDescending(e => e.Id).FirstOrDefault();
                    details.order.Address = updatedOrder.Address;
                    details.order.OrderDate = updatedOrder.OrderDate;
                    details.order.TotalPrice = updatedOrder.TotalPrice;
                    details.order.City = updatedOrder.City;
                    details.order.Email = updatedOrder.Email;
                    details.order.Name = updatedOrder.Name; // Add this line

                    details.weatherResponse = await WeatherService(updatedOrder.City);
                    //if (IsValidCoupon(couponCode))
                    //{
                    //    // Apply a fixed discount of 10 shekels
                    //    updatedOrder.TotalPrice = ApplyCoupon(updatedOrder.TotalPrice);
                    //}
                    //else
                    //{
                    //    // Invalid coupon code, add a model error
                    //    ModelState.AddModelError("CouponCode", "Invalid coupon code. Please enter a valid code.");
                       
                    //}

                    _context.SaveChanges(); // Save changes to the database
                    await SendOrderConfirmationEmail(updatedOrder);
                    // Redirect to the PayPal.html page with the total price as a query parameter
                    return Redirect($"/PayPal.html?totalPrice={updatedOrder.TotalPrice}");

                }
                else//incorrect
                {
                    ModelState.AddModelError("Address", "The provided address is incorrect. Please check the address and try again.");

                }
            }
            return View(updatedOrder);


        }



        private async Task<bool> CheckAddress(string city, string street)
        {
            var apiService = new ApiService("acc_3d60a751e375dec");
            //http://localhost:5122/api/address?CityName=%D7%91%D7%A0%D7%99%20%D7%91%D7%A8%D7%A7&StreetName=%D7%A7%D7%91%D7%95%D7%A5%20%D7%92%D7%9C%D7%99%D7%95%D7%AA

            var address = await apiService.GetApiResponseAsync<bool>($"http://gatewayservice.somee.com/api/address?CityName={Uri.EscapeDataString(city)}&StreetName={Uri.EscapeDataString(street)}");
            return address;
        }
        public async Task<WeatherResponse> WeatherService(string city)
        {
            var apiService = new ApiService("acc_3d60a751e375dec");
            var weather = await apiService.GetApiResponseAsync<WeatherResponse>($"http://gatewayService.somee.com/api/Weather?cityNmae={Uri.EscapeDataString(city)}");
            return weather;
        }

        public IActionResult GraphCreate()
        {
            return View();
        }
        public IActionResult Graph(DateTime? start, DateTime? end)
        {
            var orderCounts = new List<int>();
            var orders = _context.Order.Where(order => order.OrderDate >= start && order.OrderDate <= end).ToList();

            // Prepare data for the view model
            var dateLabels = orders.Select(order => order.OrderDate?.ToShortDateString()).Distinct().ToList();
            var totalPrices = new List<double>();

            foreach (var dateLabel in dateLabels)
            {
                orderCounts.Add(orders.Count(order => order.OrderDate?.ToShortDateString() == dateLabel));
                totalPrices.Add(orders.Where(order => order.OrderDate?.ToShortDateString() == dateLabel).Sum(order => order.TotalPrice));
            }

            var viewModel = new OrderGraphViewModel
            {
                DateLabels = dateLabels,
                TotalPrices = totalPrices,
                OrderCounts = orderCounts

            };

            return View(viewModel); // Pass the view model to the view
        }
        private async Task SendOrderConfirmationEmail(Order order)
        {
            // Replace these values with your SMTP server details
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "icepace2023@gmail.com";
            string smtpPassword = "zzkm wtti wngk uozo";

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                var message = new MailMessage
                {
                    From = new MailAddress("icepace2023@gmail.com"),
                    Subject = "Order Confirmation",
                    Body = $" Dear {order.Name},Thank you for your order. Your total price is {order.TotalPrice:C}.",
                    IsBodyHtml = false
                };

                message.To.Add(order.Email);

                await client.SendMailAsync(message);
            }

        }
        

     }

}
