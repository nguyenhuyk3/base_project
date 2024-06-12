using X.PagedList;

namespace Motel.Areas.Customer.Models
{
    public class InfoViewModel
    {
        public Motel.Models.UserAccount Owner { get; set; } = null!;
        public IPagedList<Motel.Models.Review>? ReviewsOnSite { get; set; } = null;
        public List<Motel.Models.Post> Posts { get; set; } = null!;
    }
}
