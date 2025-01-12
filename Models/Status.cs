using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public enum Status
    {
        [Display(Name = "Активно")]
        Active,
        [Display(Name = "Ожидается")]
        Pending,
        [Display(Name = "Просрочено")]
        Overdue,
        [Display(Name = "Отменено")]
        Cancelled,
        [Display(Name = "Выполнено")]
        Done
    }
}
