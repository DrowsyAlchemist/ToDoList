using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class LoginData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
        public string? UserModelId {  get; set; }
    }
}
