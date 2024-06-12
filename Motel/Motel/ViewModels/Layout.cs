using Motel.Areas.Post.Models;
using Motel.Models;

namespace Motel.ViewModels
{
    public class ModificationLayoutViewModel
    {
        public UserAccount? Owner { get; set; }
        public PostAdd? PostAdd { get; set; }
        public List<Post>? FavoritePosts { get; set; }
        public List<string>? FavoritePostIds { get; set; }
        public Dictionary<string, bool>? PeopleBooked { get; set; }
        public List<Post>? Posts { get; set; }
        public Motel.Areas.Customer.Models.Info? Info { get; set; }
        public List<Bill> Bills { get; set; }
    }
}
