using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstWebMVC.Models.ViewModels
{
    // CÁI RỔ LỚN: Chứa thông tin chung của cả Phiếu
    public class ImportSlipViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Ngày nhập")]
        public DateTime ImportDate { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        [Display(Name = "Nhà cung cấp")]
        public int SupplierId { get; set; }

        // DANH SÁCH CHI TIẾT: Chứa nhiều mặt hàng bên trong cái phiếu này
        public List<ImportSlipDetailViewModel> Details { get; set; } = new List<ImportSlipDetailViewModel>();
    }

    // CÁI RỔ NHỎ: Đại diện cho 1 dòng mặt hàng khi bro bấm nút "Thêm dòng"
    public class ImportSlipDetailViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn thiết bị")]
        public int DeviceId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập đơn giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không hợp lệ")]
        public double ImportPrice { get; set; }
    }
}