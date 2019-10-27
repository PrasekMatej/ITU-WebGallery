using System;

namespace WebGallery.BL.DTO
{
    public class DtoBase
    {
        public Guid Id { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
