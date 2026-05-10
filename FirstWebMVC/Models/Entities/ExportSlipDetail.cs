using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstWebMVC.Models.Entities
{
    public class ExportSlipDetail
    {
        [Key]
        public int Id { get; set; }
        
        public int ExportSlipId { get; set; }
        [ForeignKey("ExportSlipId")]
        public virtual ExportSlip? ExportSlip { get; set; }
        
        public int DeviceId { get; set; }
        [ForeignKey("DeviceId")]
        public virtual Device? Device { get; set; }
        
        public int Quantity { get; set; }
        public double ExportPrice { get; set; } 
        
        public double TotalPrice => Quantity * ExportPrice; 
    }
}