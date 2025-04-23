using ToDoList.DataBase;

namespace ToDoList.Models
{
    public class UserModel
    {
        public  string? Id { get; set; }
        public required string Name { get; set; }
        public required LoginData LoginData { get; set; }
        public string? Role { get; set; }
        public string? Avatar { get; set; }
        public List<TaskModel> Tasks { get; set; }
    }
}
