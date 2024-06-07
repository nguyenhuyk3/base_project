using Motel.Areas.Post.Models;
using Motel.Models;

namespace Motel.ViewModels
{
    public class ModificationLayoutViewModel
    {
        public UserAccount? Owner { get; set; }
        public PostAdd? PostAdd { get; set; }
        public List<Post>? FavoritePosts { get; set; }
        public Dictionary<string, bool>? PeopleDoNotRateYet { get; set; }
        public List<Post>? Posts { get; set; }
    }
}
