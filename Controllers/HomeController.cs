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
        //public async Task<IActionResult> Index()
        //{
        //      return _context.IceCream != null ? 
        //                  View(await _context.IceCream.ToListAsync()) :
        //                  Problem("Entity set 'Cloud1Context.IceCream'  is null.");
        //}
        // GET: About
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            //ViewData["Layout"] = layout;
            return View();
        }
        
        public async Task<IActionResult> Product()
        {
            var products = await _context.IceCream.ToListAsync();
            return View(products);
                         
        }


        //public IActionResult Single()
        //{
        //    return View();
        //}

        // Action method to view a single product
        public IActionResult Single(int id)
        {
            // Retrieve the product from the database based on the id
            var product = _context.IceCream.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); // Handle not found case
            }

            return View(product); // Pass the product to the SingleProduct view
        }

        // GET: Home/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.IceCream == null)
            {
                return NotFound();
            }

            var iceCream = await _context.IceCream
                .FirstOrDefaultAsync(m => m.Id == id);
            if (iceCream == null)
            {
                return NotFound();
            }

            return View(iceCream);
        }

        //// GET: Home/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Home/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,IceName,IceDescription,imageUrl,Calories")] IceCream iceCream)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(iceCream);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(iceCream);
        //}

        //// GET: Home/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.IceCream == null)
        //    {
        //        return NotFound();
        //    }

        //    var iceCream = await _context.IceCream.FindAsync(id);
        //    if (iceCream == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(iceCream);
        //}

        //// POST: Home/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,IceName,IceDescription,imageUrl,Calories")] IceCream iceCream)
        //{
        //    if (id != iceCream.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(iceCream);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!IceCreamExists(iceCream.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(iceCream);
        //}

        //// GET: Home/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.IceCream == null)
        //    {
        //        return NotFound();
        //    }

        //    var iceCream = await _context.IceCream
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (iceCream == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(iceCream);
        //}

        //// POST: Home/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.IceCream == null)
        //    {
        //        return Problem("Entity set 'Cloud1Context.IceCream'  is null.");
        //    }
        //    var iceCream = await _context.IceCream.FindAsync(id);
        //    if (iceCream != null)
        //    {
        //        _context.IceCream.Remove(iceCream);
        //    }
            
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool IceCreamExists(int id)
        //{
        //  return (_context.IceCream?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
    }
}
