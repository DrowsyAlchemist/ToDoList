namespace ToDoList.Models
{
    public class LoginData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? UserModelId {  get; set; }
    }
}
