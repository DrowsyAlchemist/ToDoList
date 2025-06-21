using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class TaskModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Lable { get; set; }

       // [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy hh:MM:ss}")]
        public DateTime? ExpiresDate { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public string? Description { get; set; }
        public string UserId {  get; set; }
        public UserModel? User { get; set; }
    }
}
