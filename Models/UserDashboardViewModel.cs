namespace Libereay_System.Models
{
    public class UserDashboardViewModel
    {
        public List<Book> FeaturedBooks { get; set; }
        public List<BorrowTransaction> BorrowedBooks { get; set; }
    }
}
