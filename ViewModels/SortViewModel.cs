using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class SortViewModel
    {
        public SortState LableSort { get; }
        public SortState DateSort { get; }
        public SortState StatusSort { get; }
        public SortState Current { get; }

        public SortViewModel(SortState sortState)
        {
            switch (sortState)
            {
                case SortState.LableAsc:
                    LableSort = SortState.LableDesc;
                    break;
                case SortState.LableDesc:
                    LableSort = SortState.LableAsc;
                    break;
                case SortState.DueDateAsc:
                    DateSort = SortState.DueDateDesc;
                    break;
                case SortState.DueDateDesc:
                    DateSort = SortState.DueDateAsc;
                    break;
                case SortState.StatusAsc:
                    StatusSort = SortState.StatusDesc;
                    break;
                case SortState.StatusDesc:
                    StatusSort = SortState.StatusAsc;
                    break;
            }
            Current = sortState;
        }
    }
}
