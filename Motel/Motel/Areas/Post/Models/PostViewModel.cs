namespace Motel.Areas.Post.Models
{
    public class PostAdd()
    {
        public string PostId { get; set; } = null!;
        public string CategoryId { get; set; } = null!;
        public string ApiId { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string SubjectOnSite { get; set; } = null!;
        public string Description { get; set; } = null!;
        public float SquareMeter { get; set; } = 0;
        public decimal Price { get; set; } = 0;
        public string Furniture { get; set; } = "Không nội thất";
        public int Bedroom { get; set; } = 0;
        public int Toilet { get; set; } = 0;
        public int Floor { get; set; } = 0;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string VipName { get; set; } = "v1";
    }

    public class PostDetail
    {
        public Motel.Models.Post Post { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
