using System.ComponentModel.DataAnnotations;

namespace FirstWebMVC.Models.Entities
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        public string Name { get; set; }
        
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}