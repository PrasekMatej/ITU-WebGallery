using System;

namespace WebGallery.BL.DAL
{
    public class TagEntity : EntityBase
    {
        public string Name { get; set; }
        public Guid? Owner { get; set; }
    }
}