using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class SortViewModel
    {
        public SortState LableSort { get; }
        public SortState DateSort { get; }
        public SortState PrioritySort { get; }
        public SortState Current { get; }
        public bool IsAsc => Current == SortState.LableAsc
            || Current == SortState.PriorityAsc
            || Current == SortState.DueDateAsc;

        public SortViewModel(SortState sortState)
        {
            LableSort = SortState.LableAsc;
            DateSort = SortState.DueDateDesc;
            PrioritySort = SortState.PriorityAsc;

            switch (sortState)
            {
                case SortState.LableAsc:
                    Current = LableSort = SortState.LableDesc;
                    break;
                case SortState.LableDesc:
                    Current = LableSort = SortState.LableAsc;
                    break;
                case SortState.DueDateAsc:
                    Current = DateSort = SortState.DueDateDesc;
                    break;
                case SortState.DueDateDesc:
                    Current = DateSort = SortState.DueDateAsc;
                    break;
                case SortState.PriorityAsc:
                    Current = PrioritySort = SortState.PriorityDesc;
                    break;
                case SortState.PriorityDesc:
                    Current = PrioritySort = SortState.PriorityAsc;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
