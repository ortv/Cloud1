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
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;

namespace Cloud1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly Cloud1Context _context;
        //private readonly IOptions<EmailSettings> _emailSettings;
        public OrdersController(Cloud1Context context)
        {
            _context = context;
            //_emailSettings = emailSettings;
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
        public IActionResult Update(Order updatedOrder)
        {
            // Add the order to the context (in-memory representation)
            _context.Order.Add(updatedOrder);
            //SendEmail(updatedOrder);
            // Save changes to the database
            _context.SaveChanges();

            // Redirect to the PayPal.html page with the total price as a query parameter
            return Redirect($"/PayPal.html?totalPrice={updatedOrder.TotalPrice}");

        }
        //private void SendEmail(Order order)
        //{
        //    string subject = "Order Completed";
        //    string body = $"Thank you for your order! Your total price is {order.TotalPrice.ToString("C")}. We will process your order shortly.";

        //    using (SmtpClient client = new SmtpClient(_emailSettings.Value.SmtpServer, _emailSettings.Value.Port))
        //    {
        //        client.Credentials = new NetworkCredential(_emailSettings.Value.UserName, _emailSettings.Value.Password);
        //        client.EnableSsl = true;

        //        MailMessage mailMessage = new MailMessage
        //        {
        //            From = new MailAddress(_emailSettings.Value.UserName),
        //            Subject = subject,
        //            Body = body,
        //            IsBodyHtml = true
        //        };

        //        mailMessage.To.Add(order.Email);

        //        client.Send(mailMessage);
        //    }
        //}


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


	}
}
