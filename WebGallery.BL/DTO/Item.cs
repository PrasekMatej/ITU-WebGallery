using System;

namespace WebGallery.BL.DTO
{
    public abstract class Item : DtoBase
    {
        public string Name { get; set; }
        public Guid Parent { get; set; }
    }
}
