

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Models;
using Motel.Utility.Database;
using System.Diagnostics.CodeAnalysis;

namespace Motel.Utility.Comparer
{
    public class PostComparer : IComparer<Post>
    {
        private readonly Dictionary<string, UserAccount> _ownersCache;
        private readonly Dictionary<string, int> _vipRank;

        public PostComparer(Dictionary<string, UserAccount> ownersCache)
        {
            _ownersCache = ownersCache;
            _vipRank = new Dictionary<string, int>
            {
                { "v6", 6 },
                { "v5", 5 },
                { "v4", 4 },
                { "v3", 3 },
                { "v2", 2 },
                { "v1", 1 }
            };
        }

        public int Compare(Post? x, Post? y)
        {
            int vipRankX = _vipRank.ContainsKey(x.VipName) ? _vipRank[x.VipName] : 0;
            int vipRankY = _vipRank.ContainsKey(y.VipName) ? _vipRank[y.VipName] : 0;

            if (vipRankX > vipRankY)
            {
                return -1;
            }
            else if (vipRankX < vipRankY)
            {
                return 1;
            }

            UserAccount ownerX = _ownersCache[x.OwnerId];
            UserAccount ownerY = _ownersCache[y.OwnerId];

            return ownerY.Rating.CompareTo(ownerX.Rating);
        }

        public bool Equals(Post? x, Post? y)
        {
            throw new NotImplementedException();
        }
    }
}
