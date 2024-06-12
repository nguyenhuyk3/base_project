using MongoDB.Driver;
using Motel.Models;
using Motel.Utility.Comparer;
using Motel.Utility.Database;

namespace Motel.Utility.Sort
{
    public class Post
    {
        private readonly DatabaseConstructor _databaseConstructor;

        public Post(DatabaseConstructor databaseConstructor)
        {
            _databaseConstructor = databaseConstructor;
        }

        public async Task<Dictionary<string, UserAccount>> LoadUserAccounts(List<Models.Post> posts)
        {
            var ownerIds = posts.Select(p => p.OwnerId).ToList();
            var userAccounts = await _databaseConstructor.UserAccountCollection
                                                            .Find(u => ownerIds.Contains(u.Id))
                                                            .ToListAsync();

            return userAccounts.ToDictionary(u => u.Id, u => u);
        }

        public async Task<List<Models.Post>> SortPosts(List<Models.Post> posts)
        {
            var ownersCache = await LoadUserAccounts(posts);
            var comparer = new PostComparer(ownersCache);

            posts.Sort(comparer);

            return posts;
        }
    }
}
