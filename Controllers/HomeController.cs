using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cloud1.Data;
using Cloud1.Models;

namespace Cloud1.Controllers
{
    public class HomeController : Controller
    {
        private readonly Cloud1Context _context;
        //  private static string layout = "_LayoutHeadFooter";

        public HomeController(Cloud1Context context)
        {
            _context = context;
        }

        // GET: Home
        public IActionResult Index()
        {
            return View("Index");
        }
     
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            //ViewData["Layout"] = layout;
            return View();
        }
        public IActionResult SizeSelected()
        {
            //ViewData["Layout"] = layout;
            return View();
        }


        public async Task<IActionResult> Product()
        {
            var products = await _context.IceCream1.ToListAsync();
            return View(products);

        }


        public IActionResult Single(int id)
        {
            // Retrieve the product from the database based on the id
            var product = _context.IceCream1.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); // Handle not found case
            }

            return View(product); // Pass the product to the SingleProduct view
        }

        // GET: Home/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.IceCream1 == null)
            {
                return NotFound();
            }

            var iceCream = await _context.IceCream1
                .FirstOrDefaultAsync(m => m.Id == id);
            if (iceCream == null)
            {
                return NotFound();
            }

            return View(iceCream);
        }

    }
}
