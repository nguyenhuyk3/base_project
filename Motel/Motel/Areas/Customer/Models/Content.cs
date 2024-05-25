using Motel.Models;

namespace Motel.Areas.Customer.Models
{
    public class Content
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class UnreadedNotification
    {
        public int Count { get; set; }
        public List<Notification> Notifications { get; set; }
    }
}
