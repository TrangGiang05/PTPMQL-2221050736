using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstWebMVC.Models.Entities
{
    public class ImportSlip
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime ImportDate { get; set; } = DateTime.Now;
        
        public int SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        public virtual ICollection<ImportSlipDetail>? ImportSlipDetails { get; set; }
    }
}