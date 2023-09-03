using WebApplicationFinalVersion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;

namespace WebApplicationFinalVersion.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public AdminController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult AdminProfile()
        {
            var id = HttpContext.Session.GetInt32("id");
            ViewBag.name = _context.Users.Where(x => x.Userid == id).FirstOrDefault().Username;
            ViewBag.email = _context.Users.Where(x => x.Userid == id).FirstOrDefault().Email;
            ViewBag.path = _context.Users.Where(x => x.Userid == id).FirstOrDefault().Imagepath;
            ViewBag.role = _context.Users.Where(x => x.Userid == id).FirstOrDefault().Role;
            return View();
        }
        [HttpGet]
        public IActionResult RecipesByChef()
        {

            var recipes = _context.Recipes.Where(r => r.Status.ToLower() == "wating");
            return View("RecipesByChef", recipes);
        }
        public IActionResult ChangeStatus(decimal id)
        {
            var recipe = _context.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
            }

            recipe.Status = null;
            _context.Update(recipe);
            _context.SaveChanges();

            return RedirectToAction("RecipesByChef");
        }
        public async Task<IActionResult> DeleteRecipe(decimal id)
        {
            if (_context.Recipes == null)
            {
                return Problem("Entity set 'ModelContext.Recipes'  is null.");
            }
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("RecipesByChef");
        }

        public IActionResult Dashboard()
        {
            ViewBag.users = _context.Users.Where(x => x.Role.ToLower().Equals("user")).Count();
            ViewBag.chefs = _context.Users.Where(x => x.Role.ToLower().Equals("chef")).Count();
            ViewBag.recipes = _context.Recipes.Count();
            ViewBag.recipesRequest = _context.Reciperequests.Count();
            var id = HttpContext.Session.GetInt32("id");
            ViewBag.name = _context.Users.Where(u => u.Userid == id).FirstOrDefault().Username;
            ViewBag.email = _context.Users.Where(u => u.Userid == id).FirstOrDefault().Email;
            ViewBag.path = _context.Users.Where(u => u.Userid == id).FirstOrDefault().Imagepath;


            return View();
        }
        //[HttpPost]
        public async Task<IActionResult> RecipesBySearch(DateTime? startDate, DateTime? endDate)
        {
            var recipes = _context.Recipes.Where(x => x.Creationdate >= startDate && x.Creationdate <= endDate);
            return View(recipes);
        }
        [HttpGet]
        public IActionResult Search()
        {
            var rescipes = _context.Recipes.ToList();
            return View(rescipes);
        }
        [HttpPost]
        public async Task<IActionResult> Search(DateTime? startDate, DateTime? endDate)
        {
            var recipes = _context.Recipes.ToList();
            if (startDate == null && endDate == null)
            {
                return View(recipes);
            }
            else if (startDate == null && endDate != null)
            {
                var result = recipes.Where(r => r.Creationdate.Value.Date < endDate);
                return View(result);
            }
            else if (startDate != null && endDate == null)
            {
                var result = recipes.Where(r => r.Creationdate.Value.Date > startDate);
                return View(result);
            }
            else
            {
                var result = recipes.Where(r => r.Creationdate.Value.Date > startDate && r.Creationdate.Value.Date < endDate);
                return View(result);
            }

            return RedirectToAction("Dashboard");
        }
        public IActionResult RequestedRecipes()
        {
            return RedirectToAction("Index", "Reciperequests");
        }
        public IActionResult EditContact()
        {
           
            return View();
        }
        public IActionResult EditAboutUs()
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAbout([Bind("AboutUsId,AboutUsText,AboutUsPhone,AboutUsEmail")] AboutU about)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingAbout = await _context.AboutUs.FirstOrDefaultAsync(a => a.AboutUsId == 1);
                    if (existingAbout != null)
                    {
                        existingAbout.AboutUsText = about.AboutUsText;
                        existingAbout.AboutUsPhone = about.AboutUsPhone;
                        existingAbout.AboutUsEmail = about.AboutUsEmail;

                        _context.Update(existingAbout);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(EditAbout)); // Redirect to the edit action
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency exception
                    // You might want to show an error message or handle the situation
                }
            }

            // If ModelState is not valid or an exception occurred, return back to the edit view
            return View("EditAbout", about);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContact([Bind("ContactUsId,Name,Column1,Column2")] ContactU contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingContact = await _context.ContactUs.FirstOrDefaultAsync(a => a.ContactUsId == 1);
                    if (existingContact != null)
                    {
                        
                        existingContact.Name = contact.Name;
                        existingContact.Column1 = contact.Column1;
                        existingContact.Column2 = contact.Column2;

                        _context.Update(existingContact);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(EditAbout)); // Redirect to the edit action
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
            }

            
            return View();
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            var id = HttpContext.Session.GetInt32("id");
            ViewBag.name = _context.Users.Where(x => x.Userid == id).FirstOrDefault().Username;
            ViewBag.email = _context.Users.Where(x => x.Userid == id).FirstOrDefault().Email;
            ViewBag.path = _context.Users.Where(x => x.Userid == id).FirstOrDefault().Imagepath;
            ViewBag.role = _context.Users.Where(x => x.Userid == id).FirstOrDefault().Role;
            return View();
        }
        public async Task<IActionResult> Edit([Bind("Username,Email,Password,ImageFile")] User user)
        {
            if (user.Password == null || user.Password.Length == 0)
            {
                user.Password = HttpContext.Session.GetString("password");
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                    string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                    user.Password = hashedPassword;
                }
            }
            else
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                    string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                    user.Password = hashedPassword;
                }
            }
            user.Userid = (decimal)HttpContext.Session.GetInt32("id");
            user.Role = HttpContext.Session.GetString("role");
            {
                if (user.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" +
                    user.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await user.ImageFile.CopyToAsync(fileStream);
                    }
                    user.Imagepath = fileName;
                }
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Userid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("AdminProfile");
            }
            //return View("AdminProfile", user);
        }
        private bool UserExists(decimal? id)
        {
            return (_context.Users?.Any(e => e.Userid == id)).GetValueOrDefault();
        }

    }

}

