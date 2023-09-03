using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApplicationFinalVersion.Models
{
    public partial class Testimonial
    {
        public decimal Testimonialid { get; set; }
        public decimal? Userid { get; set; }
        public string Content { get; set; }
        public virtual User User { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

    }
}
