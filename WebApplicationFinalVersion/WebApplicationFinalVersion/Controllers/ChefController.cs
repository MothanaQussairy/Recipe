using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationFinalVersion.Models;
using System.Net;
using System.Net.Mail;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.Contracts;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;

namespace WebApplicationFinalVersion.Controllers
{
    public class ChefController : Controller
    {
        private readonly ModelContext _context;
        private readonly IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnviroment;

        public ChefController(ModelContext context, IConfiguration configuration,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            this._webHostEnviroment = webHostEnvironment;
        }
        public async Task<IActionResult> RequestedRecipes()
        {
            var requestedRecipes = await _context.Reciperequests.ToListAsync();
            return View("RequestedRecipes", requestedRecipes);
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
            return RedirectToAction("RequestedRecipes");
        }



        // GET: Chef
        public IActionResult Index()
        {
            ViewBag.NumberOfRecipes = NumberOfRecipes();
            ViewBag.test = _context.Testimonials.ToList();
            ViewBag.path = HttpContext.Session.GetString("path");
            ViewBag.chefs=_context.Users.Where(x=>x.Role.ToLower()=="chef").ToList();
            ViewBag.usersWithTestimonials = _context.Users.Where(user => _context.Testimonials.Any(testimonial => testimonial.Userid == user.Userid)).ToList();
            ViewBag.requestrec=_context.Reciperequests.ToList();
            var cat = _context.Categories.ToList();
            return View(cat);
        }


        public ActionResult SendEmail(decimal id)
        {
            Reciperequest reciperequest = _context.Reciperequests.Find(id);
            reciperequest.Status = "Accepted";
            using (SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com", 587))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("hamza1082001@outlook.com", "1082001h");
                smtpClient.EnableSsl = true;

                var customerId = reciperequest.Userid;
                string customerEmail = _context.Users.Where(u => u.Userid == customerId).Select(u => u.Email).FirstOrDefault();
                var recipe = _context.Recipes.Where(r => r.Recipeid == reciperequest.Recipeid).FirstOrDefault();
                string chefName=_context.Users.Where(u=>u.Userid==recipe.Chefid).FirstOrDefault().Username;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("hamza1082001@outlook.com");
                mailMessage.To.Add(customerEmail);
                mailMessage.Subject = "Accept Recipe Request";
                mailMessage.Body = $@" Dear Customer,
                                        We are pleased to inform you that your recipe request has been accepted:
                                        Recipe: {recipe.Title}
                                        Date of Request: {reciperequest.CreatedDate}
                                        Accepted by: {chefName}
                                        Thank you for choosing our service. We look forward to serving you!
                                        Best regards,
                                        Your Team
                                        ";
                mailMessage.IsBodyHtml = true; // Set to true if you want to send HTML content
                smtpClient.Send(mailMessage);
                 
            }

             EditRquestStatus(reciperequest);
            return RedirectToAction("RequestedRecipes");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRquestStatus(Reciperequest reciperequest)
        {
            

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
            return RedirectToAction("RequestedRecipes");
        }
        private bool ReciperequestExists(decimal id)
        {
            return (_context.Reciperequests?.Any(e => e.Requestid == id)).GetValueOrDefault();
        }

        [HttpPost]
        public async Task<IActionResult> MakeRecipec([Bind("Title,Creationdate,ImageFile")] Recipe recipe, string ChefName, string CategoryName)
        {
            recipe.Status = "Wating";
            recipe.Chefid = HttpContext.Session.GetInt32("id");
           // if (recipe.ImageFile != null)
            {
                string wwwRootPath = _webHostEnviroment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" +
                recipe.ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await recipe.ImageFile.CopyToAsync(fileStream);
                }
                recipe.Imagepath = fileName;

                if(CategoryName!=null)
                recipe.Categoryid = int.Parse(CategoryName);

                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
        public IActionResult MakeRecipe()
        {
            //ViewData["Userid"] = new SelectList(_context.Users.Where(x => x.Role.ToLower() == "chef"), "Userid", "Username");
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Title");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Categoryid", "Name");
            return View();
            
        }


        public IActionResult Recipes(decimal? id)
        {
            if (id == null)
            {
                var r = _context.Recipes.ToList();
                return View(r);

            }
            var recip = _context.Recipes.Where(x => x.Chefid == id).ToList();
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
            if (string.IsNullOrEmpty(recipeName))
            {
                var allRecipes = _context.Recipes.ToList();
                return View("Recipes", allRecipes);
            }

            var matchedRecipes = _context.Recipes.Where(x => x.Title.ToLower().Contains(recipeName.ToLower())).ToList();

            ViewBag.recipes = matchedRecipes;

            return View("Recipes", matchedRecipes);
        }

        // GET: Chef/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Chef/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Chef/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Email,Password,Role,Userid,Imagepath")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Chef/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Chef/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal? id, [Bind("Username,Email,Password,Role,Userid,Imagepath")] User user)
        {
            if (id != user.Userid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Chef/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Chef/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal? id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ModelContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(decimal? id)
        {
          return (_context.Users?.Any(e => e.Userid == id)).GetValueOrDefault();
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
        //public IActionResult SendEmail()
        //{
        //    string host = _configuration["SmtpSettings:Host"];
        //    int port = int.Parse(_configuration["SmtpSettings:Port"]);
        //    string username = _configuration["SmtpSettings:Username"];
        //    string password = _configuration["SmtpSettings:Password"];
        //    bool enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]);

        //    using (SmtpClient smtpClient = new SmtpClient(host, port))
        //    {
        //        smtpClient.Credentials = new NetworkCredential(username, password);
        //        smtpClient.EnableSsl = enableSsl;

        //        using (MailMessage mailMessage = new MailMessage())
        //        {
        //            mailMessage.From = new MailAddress("your_email@example.com");
        //            mailMessage.To.Add("recipient@example.com");
        //            mailMessage.Subject = "Subject";
        //            mailMessage.Body = "Email body content";
        //            mailMessage.IsBodyHtml = true;

        //            smtpClient.Send(mailMessage);
        //        }
        //    }

        //    return RedirectToAction("Index", "Home");
        //}
    }
}




   


