using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class SortViewModel
    {
        public SortState LableSort { get; }
        public SortState DateSort { get; }
        public SortState PrioritySort { get; }
        public SortState Current { get; }

        public SortViewModel(SortState sortState)
        {
            LableSort = (sortState == SortState.LableAsc) ? SortState.LableDesc : SortState.LableAsc;
            DateSort = (sortState == SortState.DueDateAsc) ? SortState.DueDateDesc : SortState.DueDateAsc;
            PrioritySort = (sortState == SortState.PriorityAsc) ? SortState.PriorityDesc : SortState.PriorityAsc;
            Current = sortState;
        }
        public SortViewModel() { }
    }
}
