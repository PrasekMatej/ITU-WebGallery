using System;

namespace WebGallery.BL.DTO
{
    public class Photo : Item
    {
        public string Description { get; set; }
        public Tags Tags { get; set; }
        public DateTime CreatedData { get; set; }
        public string Url { get; set; }
        public string Metadata { get; set; }
    }
}
