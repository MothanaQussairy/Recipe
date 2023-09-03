using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApplicationFinalVersion.Models
{
    public partial class AboutU
    {
        public decimal AboutUsId { get; set; }
        public string AboutUsPhoto { get; set; }
        public string AboutUsText { get; set; }
        public string AboutUsPhone { get; set; }
        public string AboutUsEmail { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

    }
}
