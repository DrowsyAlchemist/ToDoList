namespace ToDoList.ViewModels
{
    public class PageViewModel
    {
        public int PageNumber { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 10;
        public int ItemsCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(ItemsCount / (double)ItemsPerPage);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PageViewModel(int itemsCount, int pageNumber, int itemsPerPage)
        {
            PageNumber = pageNumber;
            ItemsPerPage = itemsPerPage;
            ItemsCount = itemsCount;
        }

        public PageViewModel()
        {

        }
    }
}
