using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApplicationFinalVersion.Models
{
    public partial class OrderRecipe
    {
        public decimal OrderId { get; set; }
        public string OrderCreatedDate { get; set; }
        public decimal? RecipeId { get; set; }

        public virtual Recipe Recipe { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

    }
}
