using System.ComponentModel.DataAnnotations;
using ToDoList.Models.ValidationAttributes;

namespace ToDoList.Models
{
    [NamePasswordEqual]
    public class UserModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "The length of the string should be from 3 to 50 characters.")]
        public required string Name { get; set; }

        public required LoginData LoginData { get; set; }

        public string? Role { get; set; }

        public string? Avatar { get; set; }

        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();
    }
}
