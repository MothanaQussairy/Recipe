using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApplicationFinalVersion.Models
{
    public partial class ContactU
    {
        public decimal ContactUsId { get; set; }
        public string Name { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

    }
}
