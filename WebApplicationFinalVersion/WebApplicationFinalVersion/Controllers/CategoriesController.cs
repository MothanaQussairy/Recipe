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
    public class CategoriesController : Controller
    {
        private readonly ModelContext _context;

        public CategoriesController(ModelContext context)
        {
            _context = context;
        }
        [NonAction]
        public List<int> NumberOfRecipes()
        {
            List<int> noOfRecipes = new List<int>();
            foreach (var i in _context.Categories)
            {
                noOfRecipes.Add(_context.Recipes.Where(x => x.Categoryid == i.Categoryid).Count());
            }
            return noOfRecipes;
        }
         
        // GET: Categories
        public async Task<IActionResult> Index()
        {
            ViewBag.users = _context.Users.Where(x => x.Role.ToLower().Equals("user")).Count();
            ViewBag.chefs = _context.Users.Where(x => x.Role.ToLower().Equals("chef")).Count();
            ViewBag.recipes = _context.Recipes.Count();
            ViewBag.recipesRequest = _context.Reciperequests.Count();
            var id = HttpContext.Session.GetInt32("id");
            ViewBag.name = _context.Users.Where(u => u.Userid == id).FirstOrDefault().Username;
            ViewBag.email = _context.Users.Where(u => u.Userid == id).FirstOrDefault().Email;
            ViewBag.path = _context.Users.Where(u => u.Userid == id).FirstOrDefault().Imagepath;
            ViewBag.noOfRecipes=NumberOfRecipes();
            RedirectToAction("Create","Recipes");
            return _context.Categories != null ? 
                          View(await _context.Categories.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Categories'  is null.");
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Categoryid == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Categoryid")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Name,Categoryid")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Categoryid))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Categoryid == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ModelContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(decimal id)
        {
          return (_context.Categories?.Any(e => e.Categoryid == id)).GetValueOrDefault();
        }
    }
}
