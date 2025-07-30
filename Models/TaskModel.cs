using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class TaskModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Lable is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "The length of the string should be from 3 to 50 characters.")]
        public required string Lable { get; set; }
        public DateTime ExpiresDate { get; set; }
        public TasksStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string? Description { get; set; }

        [ValidateNever]
        public string? UserId {  get; set; }

        [ValidateNever]
        public UserModel? User { get; set; }
    }
}
