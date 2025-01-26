namespace ToDoList.Models
{
    public class UserModel
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required LoginData LoginData { get; set; }
        public string? Avatar { get; set; }
    }
}
