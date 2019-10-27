using System;
using System.Collections.Generic;
using WebGallery.BL.DTO;

namespace WebGallery.ViewModels
{
    public class PhotoDetailViewModel : AuthenticatedMasterPageViewModel
    {
        public List<Photo> Photos { get; set; } = new List<Photo>()
        {
            new Photo()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Name = "aa",
                Url = "https://picsum.photos/id/819/1920/1080",
                Description = "desc"
            },
            new Photo()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Name = "bb",
                Url = "https://picsum.photos/id/822/1920/1080",
                Description = "desc"
            },
            new Photo()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Name = "c",
                Url = "https://picsum.photos/id/154/1920/1080",
                Description = "desc"
            }
        };
        
        public Photo OpenedInfo { get; set; }
    }
}

