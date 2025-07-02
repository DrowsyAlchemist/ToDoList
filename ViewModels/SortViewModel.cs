using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class SortViewModel
    {
        public SortState LableSort { get; }
        public SortState DateSort { get; }
        public SortState PrioritySort { get; }
        public SortState Current { get; }
        public SortState Next { get; }
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
                    Next = LableSort = SortState.LableDesc;
                    break;
                case SortState.LableDesc:
                    Next = LableSort = SortState.LableAsc;
                    break;
                case SortState.DueDateAsc:
                    Next = DateSort = SortState.DueDateDesc;
                    break;
                case SortState.DueDateDesc:
                    Next = DateSort = SortState.DueDateAsc;
                    break;
                case SortState.PriorityAsc:
                    Next = PrioritySort = SortState.PriorityDesc;
                    break;
                case SortState.PriorityDesc:
                    Next = PrioritySort = SortState.PriorityAsc;
                    break;
                default:
                    throw new NotImplementedException();
            }
            Current = sortState;
        }
    }
}
