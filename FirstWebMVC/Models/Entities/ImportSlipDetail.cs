using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstWebMVC.Models.Entities
{
    public class ImportSlipDetail
    {
        [Key]
        public int Id { get; set; }
        
        public int ImportSlipId { get; set; }
        [ForeignKey("ImportSlipId")]
        public virtual ImportSlip? ImportSlip { get; set; }
        
        public int DeviceId { get; set; }
        [ForeignKey("DeviceId")]
        public virtual Device? Device { get; set; }
        
        public int Quantity { get; set; }
        public double ImportPrice { get; set; } 
        
        public double TotalPrice => Quantity * ImportPrice; 
    }
}