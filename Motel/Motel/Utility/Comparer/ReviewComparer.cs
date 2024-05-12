using Motel.Models;
using System.Diagnostics.CodeAnalysis;

namespace Motel.Utility.Comparer
{
    public class ReviewComparer : IEqualityComparer<Review>
    {
        public bool Equals(Review? x, Review? y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] Review obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
