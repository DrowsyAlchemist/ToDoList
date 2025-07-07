using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ToDoList.Models;

namespace ToDoList.ViewModels
{
    [Serializable]
    public class FilterViewModel
    {
        public string LablePart { get; set; } = string.Empty;
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        public SelectList Data { get; set; }

        public ViewDateScope SelectedDateScope { get; set; } = ViewDateScope.All;
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        public SelectList Status { get; set; }
        public TasksStatus? SelectedStatus { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        public SelectList Priority { get; set; }
        public Priority? SelectedPriority { get; set; }

        public FilterViewModel()
        {
            string[] viewDateScopes = Enum.GetNames(typeof(ViewDateScope));
            var viewDateScopesList = viewDateScopes.ToList();
            Data = new SelectList(viewDateScopesList);

            string[] tasksStatuses = Enum.GetNames(typeof(TasksStatus));
            var tasksStatusesList = tasksStatuses.ToList();
            tasksStatusesList.Insert(0, "All");
            Status = new SelectList(tasksStatusesList);

            string[] priorities = Enum.GetNames(typeof(Priority));
            var prioritiesList = priorities.ToList();
            prioritiesList.Insert(0, "All");
            Priority = new SelectList(prioritiesList);
        }
    }
}
