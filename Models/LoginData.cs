using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class LoginData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public required string Email { get; set; }

        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()_+=]).{8,}$",
            ErrorMessage = "The password must have:\r\n" +
            "- at least 8 characters;\r\n- at least one number; \r\n" +
            "- at least one lowercase letter; \r\n" +
            "- at least one capital letter; \r\n" +
            "- at least one special character (for example, @$!%*?&).")]
        public required string Password { get; set; }

        public string? UserModelId { get; set; }
    }
}
