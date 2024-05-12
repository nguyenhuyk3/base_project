using X.PagedList;

namespace Motel.Areas.Customer.Models
{
    public class InfoViewModel
    {
        public Motel.Models.UserAccount Owner {  get; set; }
        public string SenderId {  get; set; }
        public IPagedList<Motel.Models.Review>? ReviewsOnSite { get; set; } = null;
    }
}
