using System;

namespace WebGallery.BL.DAL
{
    public abstract class ItemEntity : EntityBase
    {
        public string Name { get; set; }
        public DirectoryEntity Parent { get; set; }
    }
}