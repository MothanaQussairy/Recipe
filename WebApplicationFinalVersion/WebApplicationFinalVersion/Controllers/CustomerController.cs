using WebApplicationFinalVersion.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;

namespace WebApplicationFinalVersion.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public CustomerController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public List<int> NumberOfRecipes()
        {
            List<int> noOfRecipes = new List<int>();
            foreach (var i in _context.Categories)
            {
                noOfRecipes.Add(_context.Recipes.Where(x => x.Categoryid == i.Categoryid).Count());
            }
            return noOfRecipes;
        }
        public IActionResult Home()
        {
            ViewBag.NumberOfRecipes = NumberOfRecipes();
            ViewBag.test=_context.Testimonials.ToList();
            ViewBag.path = HttpContext.Session.GetString("path");
            ViewBag.usersWithTestimonials = _context.Users.Where(user => _context.Testimonials.Any(testimonial => testimonial.Userid == user.Userid)).ToList();
            var cat =_context.Categories.ToList();
            return View(cat);
        }
        public IActionResult EditProfile()
        {
            ViewBag.path = HttpContext.Session.GetString("path");
            ViewBag.name = HttpContext.Session.GetString("name");
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.password = HttpContext.Session.GetString("password");
            ViewBag.role = HttpContext.Session.GetString("role");
            return View();
        }
		public IActionResult About()
		{
            ViewBag.path = HttpContext.Session.GetString("path");
            ViewBag.description = _context.AboutUs.FirstOrDefault().AboutUsText;
            ViewBag.phone= _context.AboutUs.FirstOrDefault().AboutUsPhone;
            ViewBag.email = _context.AboutUs.FirstOrDefault().AboutUsEmail;
            return View();
		}
		public IActionResult Recipes(decimal? id)
        {
            ViewBag.path = HttpContext.Session.GetString("path");

                if (id== null)
                {
                    var r=_context.Recipes.ToList();
                    return View(r);

                }
                var recip = _context.Recipes.Where(x => x.Categoryid == id).ToList();
                List<IQueryable> chefsNames = new List<IQueryable>();
                foreach (var i in recip)
                {
                    chefsNames.Add(_context.Users.Where(x => x.Userid == i.Chefid));
                }
                ViewBag.CID = chefsNames;
                return View(recip);
        }
        [HttpPost]
        public IActionResult SearchByName(string recipeName)
        {
            var allRecipes = _context.Recipes.ToList();

            if (string.IsNullOrEmpty(recipeName))
            {
                return View("Recipes", allRecipes);
            }

            var matchedRecipes = allRecipes
                .Where(x => x.Title.Contains(recipeName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matchedRecipes.Count == 0)
            {
                return RedirectToAction("Recipes", "Customer");
            }

            ViewBag.recipes = matchedRecipes;
            return View("Recipes", matchedRecipes);
        }




        public IActionResult Chefs()
        {
            ViewBag.path = HttpContext.Session.GetString("path");
            var chefs =_context.Users.Where(x => x.Role.ToLower() == "chef");
            return View(chefs);
        }
        public IActionResult ChefRecipes(decimal id)
        {
            var recip = _context.Recipes.Where(x => x.Chefid == id).ToList();
            return View(recip);
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var id = HttpContext.Session.GetInt32("id");
            ViewBag.name = _context.Users.Where(x=>x.Userid==id).FirstOrDefault().Username;
            ViewBag.email = _context.Users.Where(x => x.Userid == id).FirstOrDefault().Email;
            ViewBag.path= _context.Users.Where(x => x.Userid == id).FirstOrDefault().Imagepath;
            ViewBag.role = _context.Users.Where(x => x.Userid == id).FirstOrDefault().Role;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            user.Userid =(decimal)HttpContext.Session.GetInt32("id");
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
                return RedirectToAction("Profile");
            }
            
        }
        private bool UserExists(decimal? id)
        {
            return (_context.Users?.Any(e => e.Userid == id)).GetValueOrDefault();
        }
        public IActionResult Contact()
        {
            ViewBag.location = _context.ContactUs.FirstOrDefault().Name;
            ViewBag.email = _context.ContactUs.FirstOrDefault().Column1;
            ViewBag.phone = _context.ContactUs.FirstOrDefault().Column2;
            return View();
        }
        public IActionResult RequestedRecipesFromChef()
        {
            ViewBag.path = HttpContext.Session.GetString("path");

            ViewBag.id = HttpContext.Session.GetInt32("id");
            var r=_context.Reciperequests.ToList();
            return View(r);
        }
        public async Task<IActionResult> DeleteRequestedRecipe(decimal? id)
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
            return RedirectToAction("RequestedRecipesFromChef");
        }

        

    }
}
