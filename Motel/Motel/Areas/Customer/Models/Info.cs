namespace Motel.Areas.Customer.Models
{
    public class Info
    {
        public string OwnerId { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public bool Sex { get; set; }
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
