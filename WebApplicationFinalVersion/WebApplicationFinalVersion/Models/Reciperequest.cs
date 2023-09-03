using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApplicationFinalVersion.Models
{
    public partial class Reciperequest
    {
        public decimal Requestid { get; set; }
        public decimal? Userid { get; set; }
        public string Ingredients { get; set; }
        public decimal? Preparationtime { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal? Cost { get; set; }
        public decimal Recipeid { get; set; }
        public string Imagepath { get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual User User { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

    }
}
