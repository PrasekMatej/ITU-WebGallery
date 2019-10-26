using System.Collections.Generic;

namespace WebGallery.BL.DAL
{
    public class DirectoryEntity : ItemEntity
    {
        public virtual ICollection<ItemEntity> Items { get; set; }
    }
}