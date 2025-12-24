using System.ComponentModel.DataAnnotations;

namespace Contract;

public enum InventoryEditStatusType
{
    [Display(Name = "Chưa chỉnh sửa")]
    NotEdited = 0,

    [Display(Name = "Đã chỉnh sửa")]
    Edited = 1
}
