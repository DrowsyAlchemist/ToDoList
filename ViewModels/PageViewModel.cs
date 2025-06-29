namespace ToDoList.ViewModels
{
    public class PageViewModel
    {
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int ItemsPerPage {  get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PageViewModel(int itemsCount, int pageNumber, int itemsPerPage)
        {
            PageNumber = pageNumber;
            ItemsPerPage = itemsPerPage;
            TotalPages = (int)Math.Ceiling(itemsCount / (double)itemsPerPage);
        }
    }
}
