using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models.ValidationAttributes
{
    public class NamePasswordEqualAttribute : ValidationAttribute
    {
        public NamePasswordEqualAttribute()
        {
            ErrorMessage = "The name/email and password must not match!";
        }

        public override bool IsValid(object? value)
        {
            var user = value as UserModel;
            return user != null && user.Name != user.LoginData.Password && user.LoginData.Email != user.LoginData.Password;
        }
    }
}
