using System;
using System.Collections.Generic;

namespace WebGallery.BL.DAL
{
    public class PhotoEntity : ItemEntity
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy { get; set; }
        public string Metadata { get; set; }
        public bool CreatedDate { get; set; }
        public ICollection<TagEntity> Tags { get; set; }
    }
}