using System.Collections.Generic;

namespace WebGallery.BL.DTO
{
    public class Folder : Item
    {
        public virtual ICollection<Item> Items { get; set; }
    }
}
