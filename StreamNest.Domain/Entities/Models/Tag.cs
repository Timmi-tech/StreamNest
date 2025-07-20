namespace StreamNest.Domain.Entities.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<VideoTag> VideoTags { get; set; } = new(); 

    }

}