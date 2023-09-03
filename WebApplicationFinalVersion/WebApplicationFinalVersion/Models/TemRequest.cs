using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApplicationFinalVersion.Models
{
    public partial class TemRequest
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public decimal? ChefId { get; set; }

        public virtual User Chef { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

    }
}
