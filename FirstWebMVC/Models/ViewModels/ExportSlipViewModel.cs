using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstWebMVC.Models.ViewModels
{
    public class ExportSlipViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Ngày xuất")]
        public DateTime ExportDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Người nhận hàng")]
        public string? ReceiverName { get; set; }

        public List<ExportSlipDetailViewModel> Details { get; set; } = new List<ExportSlipDetailViewModel>();
    }

    public class ExportSlipDetailViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn thiết bị")]
        public int DeviceId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải > 0")]
        public int Quantity { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập đơn giá xuất")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không hợp lệ")]
        public double ExportPrice { get; set; }
    }
}