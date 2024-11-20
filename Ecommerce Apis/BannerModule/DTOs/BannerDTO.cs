namespace Ecommerce_Apis.CartModule.DTOs
{
    public class BannerDTO
    {
        public int Id { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Link { get; set; }
        public int CouponId { get; set; }
        public string Image { get; set; }

    } 
    public class BannerRequest
    {
        public int Link { get; set; }

        public string title { get; set; }
        public string description { get; set; }

        public int CouponId { get; set; }
        public IFormFile Image { get; set; }
    }
    public class BannerRequestDB
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public int CouponId { get; set; }
        public  int Link { get; set; }
        public  string Image { get; set; }
        public BannerRequestDB(string title,string description, int link, string image,int couponid)
        {
            Title = title;
            Description = description;
            Link = link;
            CouponId = couponid;
            Image = image;
        }
    }
}
