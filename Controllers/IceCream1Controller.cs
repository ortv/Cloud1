﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cloud1.Data;
using Cloud1.Models;
using Cloud1.Services;
using Cloud1.Migrations;

namespace Cloud1.Controllers
{
    public class IceCream1Controller : Controller
    {
        private readonly Cloud1Context _context;
        //private readonly ImaggaService imaggaService= ;

        public IceCream1Controller(Cloud1Context context)
        {
            _context = context;
            
        }

        // GET: IceCream1
        public async Task<IActionResult> Index(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                var dataContext = _context.IceCream1;
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var searchItems = await _context.IceCream1
                    .Where(s => s.IceName.Contains(searchString) || s.IceDescription.Contains(searchString))
                    .ToListAsync();
                return View(searchItems);
            }
        }

        // GET: IceCream1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.IceCream1 == null)
            {
                return NotFound();
            }

            var iceCream1 = await _context.IceCream1
                .FirstOrDefaultAsync(m => m.Id == id);
            if (iceCream1 == null)
            {
                return NotFound();
            }

            return View(iceCream1);
        }

        // GET: IceCream1/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IceCream1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IceName,IceDescription,Price,imageUrl,isAvailable")] IceCream1 iceCream1)
        {
            if (ModelState.IsValid)
            {
                _context.Add(iceCream1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(iceCream1);
        }
        private async Task<bool> CheckImage(string ImageUrl)
        {
            var apiService = new ApiService("acc_3d60a751e375dec");
            var IsIceCream = await apiService.GetApiResponseAsync<bool>($"http://gatewayService.somee.com/api/imagga?imageUrl={Uri.EscapeDataString(ImageUrl)}");
            return IsIceCream;
        }

        // GET: IceCream1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.IceCream1 == null)
            {
                return NotFound();
            }

            var iceCream1 = await _context.IceCream1.FindAsync(id);
            if (iceCream1 == null)
            {
                return NotFound();
            }
            return View(iceCream1);
        }

        // POST: IceCream1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IceName,IceDescription,Price,imageUrl,isAvailable")] IceCream1 iceCream1)
        {
            if (id != iceCream1.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(iceCream1);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IceCream1Exists(iceCream1.Id))
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
            return View(iceCream1);
        }

        // GET: IceCream1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.IceCream1 == null)
            {
                return NotFound();
            }

            var iceCream1 = await _context.IceCream1
                .FirstOrDefaultAsync(m => m.Id == id);
            if (iceCream1 == null)
            {
                return NotFound();
            }

            return View(iceCream1);
        }

        // POST: IceCream1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.IceCream1 == null)
            {
                return Problem("Entity set 'Cloud1Context.IceCream1'  is null.");
            }
            var iceCream1 = await _context.IceCream1.FindAsync(id);
            if (iceCream1 != null)
            {
                _context.IceCream1.Remove(iceCream1);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IceCream1Exists(int id)
        {
          return (_context.IceCream1?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<ActionResult> Search(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                var dataContext = _context.IceCream1;
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var searchItems = await _context.IceCream1
                    .Where(s => s.IceName.Contains(searchString) || s.IceDescription.Contains(searchString))
                    .ToListAsync();
                return View(searchItems);
            }
        }



    }
}
