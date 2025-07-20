namespace StreamNest.Domain.Entities.Models
{
    public class VideoTag
    {
        public Guid VideoId { get; set; }
        public Video? Video { get; set; }

        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }

}