namespace Libereay_System.Models
{
    public class BrowseBooksViewModel
    {
        public List<Book> Books { get; set; }
        public string SearchQuery { get; set; }
        public string SelectedCategory { get; set; }
    }
}
