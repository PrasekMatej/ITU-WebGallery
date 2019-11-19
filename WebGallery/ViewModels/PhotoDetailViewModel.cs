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
                Url = "https://picsum.photos/id/819/2560/1440",
                Description = "desc"
            },
            new Photo()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Name = "bb",
                Url = "https://picsum.photos/id/822/2560/1440",
                Description = "desc"
            },
            new Photo()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Name = "c",
                Url = "https://picsum.photos/id/154/2560/1440",
                Description = "desc"
            }
        };

        public Photo OpenedInfo { get; set; }
        public bool EditMode { get; set; }

        public void SaveDetails()
        {
            var updated = Photos.Find(photo => photo.Id == OpenedInfo.Id);
            updated.Name = OpenedInfo.Name;
            updated.Description = OpenedInfo.Description;
            EditMode = false;
        }

        public void CancelChanges()
        {
            EditMode = false;
            var openedInfoId = OpenedInfo.Id;
            OpenedInfo = null;
            OpenedInfo = Photos.Find(photo => photo.Id == openedInfoId);
        }
    }
}

