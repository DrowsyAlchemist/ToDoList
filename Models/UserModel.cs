namespace ToDoList.Models
{
    public class UserModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Name { get; set; }
        public LoginData LoginData { get; set; }
        public required string Role { get; set; }
        public string? Avatar { get; set; }
        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();
    }
}
