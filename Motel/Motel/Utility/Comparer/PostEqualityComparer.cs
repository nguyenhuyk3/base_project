using Motel.Models;
using System.Diagnostics.CodeAnalysis;

namespace Motel.Utility.Comparer
{
    public class PostEqualityComparer : IEqualityComparer<Post>
    {
        public bool Equals(Post? x, Post? y)
        {
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            return x.Id == y.Id;  // So sánh dựa trên thuộc tính Id
        }

        public int GetHashCode([DisallowNull] Post post)
        {
            if (Object.ReferenceEquals(post, null))
            {
                return 0;
            }

            return post.Id == null ? 0 : post.Id.GetHashCode();
        }
    }
}
