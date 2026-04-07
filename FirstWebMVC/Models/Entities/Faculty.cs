using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstWebMVC.Models.Entities
{
    [Table("Faculties")]
    public class Faculty
    {
        [Key]
        [Required(ErrorMessage = "Mã khoa không được để trống!")]
        public string FacultyID { get; set; } 

        [Required(ErrorMessage = "Tên khoa không được để trống!")]
        [StringLength(100)]
        public string FacultyName { get; set; }

        // Navigation Property: Một khoa có nhiều sinh viên (1-N)
        public virtual ICollection<Student> Students { get; set; } 
    }
}