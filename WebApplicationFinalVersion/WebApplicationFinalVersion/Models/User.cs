using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApplicationFinalVersion.Models
{
    public partial class User
    {
        public User()
        {
            Reciperequests = new HashSet<Reciperequest>();
            Recipes = new HashSet<Recipe>();
            TemRequests = new HashSet<TemRequest>();
            Testimonials = new HashSet<Testimonial>();
        }

        public decimal Userid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Imagepath { get; set; }

        public virtual ICollection<Reciperequest> Reciperequests { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<TemRequest> TemRequests { get; set; }
        public virtual ICollection<Testimonial> Testimonials { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

    }
}
