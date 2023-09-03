using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApplicationFinalVersion.Models
{
    public partial class Payment
    {
        public decimal PaymentId { get; set; }
        public string CardNumber { get; set; }
        public string Column1 { get; set; }
        public string CardCvc { get; set; }
        public decimal? Amount { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

    }
}
