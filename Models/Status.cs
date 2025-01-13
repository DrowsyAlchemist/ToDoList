using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public enum Status
    {
        Active,
        Pending,
        Overdue,
        Cancelled,
        Done
    }
}
