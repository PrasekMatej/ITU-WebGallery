using System;

namespace WebGallery.BL.DTO
{
    public class Tags : DtoBase
    {
        public string Name { get; set; }
        public Guid Owner { get; set; }
    }
}