using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationFinalVersion.Models;

namespace WebApplicationFinalVersion.Controllers
{
    public class TemRequestsController : Controller
    {
        private readonly ModelContext _context;

        public TemRequestsController(ModelContext context)
        {
            _context = context;
        }

        // GET: TemRequests
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.TemRequests.Include(t => t.Chef);
            return View(await modelContext.ToListAsync());
        }

        // GET: TemRequests/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.TemRequests == null)
            {
                return NotFound();
            }

            var temRequest = await _context.TemRequests
                .Include(t => t.Chef)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (temRequest == null)
            {
                return NotFound();
            }

            return View(temRequest);
        }

        // GET: TemRequests/Create
        public IActionResult Create()
        {
            ViewData["ChefId"] = new SelectList(_context.Users, "Userid", "Email");
            return View();
        }

        // POST: TemRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ChefId,Id")] TemRequest temRequest)
        {
            temRequest.ChefId= HttpContext.Session.GetInt32("id");
            if (ModelState.IsValid)
            {
                _context.Add(temRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction("MakeRecipe", "Chef");
            }
            ViewData["ChefId"] = new SelectList(_context.Users, "Userid", "Email", temRequest.ChefId);
            return View(temRequest);
        }

        // GET: TemRequests/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.TemRequests == null)
            {
                return NotFound();
            }

            var temRequest = await _context.TemRequests.FindAsync(id);
            if (temRequest == null)
            {
                return NotFound();
            }
            ViewData["ChefId"] = new SelectList(_context.Users, "Userid", "Email", temRequest.ChefId);
            return View(temRequest);
        }

        // POST: TemRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Name,ChefId,Id")] TemRequest temRequest)
        {
            if (id != temRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(temRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TemRequestExists(temRequest.Id))
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
            ViewData["ChefId"] = new SelectList(_context.Users, "Userid", "Email", temRequest.ChefId);
            return View(temRequest);
        }

        // GET: TemRequests/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.TemRequests == null)
            {
                return NotFound();
            }

            var temRequest = await _context.TemRequests
                .Include(t => t.Chef)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (temRequest == null)
            {
                return NotFound();
            }

            return View(temRequest);
        }

        // POST: TemRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.TemRequests == null)
            {
                return Problem("Entity set 'ModelContext.TemRequests'  is null.");
            }
            var temRequest = await _context.TemRequests.FindAsync(id);
            if (temRequest != null)
            {
                _context.TemRequests.Remove(temRequest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TemRequestExists(decimal id)
        {
          return (_context.TemRequests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
