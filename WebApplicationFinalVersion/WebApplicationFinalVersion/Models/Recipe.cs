using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApplicationFinalVersion.Models
{
    public partial class Recipe
    {
        public Recipe()
        {
            OrderRecipes = new HashSet<OrderRecipe>();
            Reciperequests = new HashSet<Reciperequest>();
        }

        public decimal Recipeid { get; set; }
        public string Title { get; set; }
        public DateTime? Creationdate { get; set; }
        public decimal? Categoryid { get; set; }
        public decimal? Chefid { get; set; }
        public string Imagepath { get; set; }
        public string Status { get; set; }

        public virtual Category Category { get; set; }
        public virtual User Chef { get; set; }
        public virtual ICollection<OrderRecipe> OrderRecipes { get; set; }
        public virtual ICollection<Reciperequest> Reciperequests { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

    }
}
