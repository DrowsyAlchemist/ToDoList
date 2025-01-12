using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public enum Priority
    {
        [Display(Name = "Низкий")]
        Low,
        [Display(Name = "Средний")]
        Medium,
        [Display(Name = "Высокий")]
        High
    }
}