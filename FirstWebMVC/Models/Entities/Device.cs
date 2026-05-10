using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstWebMVC.Models.Entities
{
    public class Device
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên thiết bị không được để trống")]
        public string DeviceName { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn loại thiết bị")]
        public int CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}