using System;

namespace WebGallery.BL.DTO
{
    public class Tag : DtoBase
    {
        public string Name { get; set; }
        public Guid Owner { get; set; }
    }
}