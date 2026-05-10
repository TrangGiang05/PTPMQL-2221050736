using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstWebMVC.Models.Entities
{
    public class ExportSlip
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime ExportDate { get; set; } = DateTime.Now;
        
        public string? ReceiverName { get; set; }

        public virtual ICollection<ExportSlipDetail>? ExportSlipDetails { get; set; }
    }
}