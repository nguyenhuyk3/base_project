using Motel.Models;

namespace Motel.Areas.Customer.Models
{
    public class Response
    {
        public int? Rating { get; set; } = null;
        public string Content { get; set; } = null!;
        public string? Avatar { get; set; } = null;
    }

    public class UnreadedNotification
    {
        public int Count { get; set; }
        public List<Notification> Notifications { get; set; }
    }
}
