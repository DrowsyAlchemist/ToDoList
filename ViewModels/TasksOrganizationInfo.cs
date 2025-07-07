using System.Text.Json.Serialization;
using ToDoList.Models;

namespace ToDoList.ViewModels
{
    [Serializable]
    public class TasksOrganizationInfo
    {
        public FilterViewModel? FilterViewModel { get; set; }
        public SortState SortState { get; set; } = SortState.DueDateAsc;
        public PageViewModel? PageViewModel { get; set; }
    }
}
