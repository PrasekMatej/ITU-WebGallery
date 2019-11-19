using System;
using System.Collections.Generic;

namespace WebGallery.BL.DAL
{
    public class PhotoEntity : ItemEntity
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public string Metadata { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}