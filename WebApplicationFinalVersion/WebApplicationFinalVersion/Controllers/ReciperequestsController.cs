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
    public class ReciperequestsController : Controller
    {
        private readonly ModelContext _context;

        public ReciperequestsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Reciperequests
        public async Task<IActionResult> RequestedRecipes()
        {
          
            var id = HttpContext.Session.GetInt32("id");
            ViewBag.name = _context.Users.Where(u => u.Userid == id).FirstOrDefault().Username;
            ViewBag.path = _context.Users.Where(u => u.Userid == id).FirstOrDefault().Imagepath;

            var recipeRequests = await _context.Reciperequests.ToListAsync();
            return View("Index", recipeRequests);
        }


        // GET: Reciperequests/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Reciperequests == null)
            {
                return NotFound();
            }

            var reciperequest = await _context.Reciperequests
                .Include(r => r.Recipe)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Requestid == id);
            if (reciperequest == null)
            {
                return NotFound();
            }

            return View(reciperequest);
        }

        // GET: Reciperequests/Create
        public IActionResult Create()
        {
            ViewBag.path = HttpContext.Session.GetString("path");

            ViewData["Userid"] = new SelectList(_context.Users.Where(x=>x.Role.ToLower()=="chef"), "Userid", "Username");
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Title");
            return View();
        }

        // POST: Reciperequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ingredients,Preparationtime,Status,Cost,Recipeid,Requestid,Imagepath")] Reciperequest reciperequest)
        {
            Random random = new Random();
            reciperequest.Userid = HttpContext.Session.GetInt32("id");
            reciperequest.Cost= random.Next(100, 201);
            reciperequest.CreatedDate= DateTime.Now;
            reciperequest.Imagepath = _context.Recipes.Where(x => x.Recipeid == reciperequest.Recipeid).FirstOrDefault().Imagepath;
            reciperequest.Status = "Wating";
            //if (ModelState.IsValid)
            {
                _context.Add(reciperequest);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create");
            }
        }

        // GET: Reciperequests/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Reciperequests == null)
            {
                return NotFound();
            }

            var reciperequest = await _context.Reciperequests.FindAsync(id);
            if (reciperequest == null)
            {
                return NotFound();
            }
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Recipeid", reciperequest.Recipeid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Email", reciperequest.Userid);
            return View(reciperequest);
        }

        // POST: Reciperequests/Edit/5
        // To
        // protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Userid,Ingredients,Preparationtime,Status,CreatedDate,Cost,Recipeid,Requestid,Imagepath")] Reciperequest reciperequest)
        {
            if (id != reciperequest.Requestid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reciperequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReciperequestExists(reciperequest.Requestid))
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
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Recipeid", reciperequest.Recipeid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Email", reciperequest.Userid);
            return View(reciperequest);
        }

        // GET: Reciperequests/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Reciperequests == null)
            {
                return NotFound();
            }

            var reciperequest = await _context.Reciperequests
                .Include(r => r.Recipe)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Requestid == id);
            if (reciperequest == null)
            {
                return NotFound();
            }

            return View(reciperequest);
        }

        // POST: Reciperequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Reciperequests == null)
            {
                return Problem("Entity set 'ModelContext.Reciperequests'  is null.");
            }
            var reciperequest = await _context.Reciperequests.FindAsync(id);
            if (reciperequest != null)
            {
                _context.Reciperequests.Remove(reciperequest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReciperequestExists(decimal id)
        {
          return (_context.Reciperequests?.Any(e => e.Requestid == id)).GetValueOrDefault();
        }
    }
}
