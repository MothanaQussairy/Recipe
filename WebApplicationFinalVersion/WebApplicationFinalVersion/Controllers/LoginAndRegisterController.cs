using WebApplicationFinalVersion.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationFinalVersion.Controllers
{
    public class LoginAndRegisterController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public LoginAndRegisterController(ModelContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnviroment = hostEnvironment;
        }
        public ActionResult Index() { return View(); }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> LoginUser([Bind("Username,Password")] User user)
        {
            var auth = await _context.Users.FirstOrDefaultAsync(x => x.Username == user.Username);

            if (auth != null)
            {
                // Validate the password
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                    string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                    if (hashedPassword == auth.Password)
                    {
                        HttpContext.Session.SetString("name", auth.Username);
                        HttpContext.Session.SetString("email", auth.Email);
                        HttpContext.Session.SetString("path", auth.Imagepath);
                        HttpContext.Session.SetString("role", auth.Role);
                        HttpContext.Session.SetInt32("id", (int)auth.Userid);
                        HttpContext.Session.SetString("password", user.Password);


                        if (auth.Role.ToLower() == "admin")
                            return RedirectToAction("Dashboard", "Admin");
                        else if (auth.Role.ToLower() == "user")
                            return RedirectToAction("Home", "Customer");
                        else
                            return RedirectToAction("Index", "Chef");
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserRegister([Bind("Username,Email,Password,Userid,ImageFile")] User user)
        {
            user.Role = "User";
            if (user.ImageFile != null)
            {
                string imageFileName = await SaveImageFile(user.ImageFile);
                user.Imagepath = imageFileName;
            }

            // Hash the password before storing it
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                user.Password = hashedPassword;
            }

            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task<string> SaveImageFile(IFormFile imageFile)
        {
            string wwwRootPath = _webHostEnviroment.WebRootPath;
            string fileName = GenerateUniqueFileName(imageFile.FileName);
            string imagePath = Path.Combine(wwwRootPath, "Image", fileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return fileName;
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            string uniqueFileName = $"{Guid.NewGuid():N}_{originalFileName}";
            return uniqueFileName;
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
