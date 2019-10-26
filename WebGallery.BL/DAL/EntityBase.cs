using System;

namespace WebGallery.BL.DAL
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}