using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.BusinessPack.Filters;
using DotVVM.Framework.ViewModel;
using WebGallery.BL.DTO;
using WebGallery.BL.Services;

namespace WebGallery.ViewModels
{
    public class CarouselPhotoItem
    {
        public Photo Photo { get; set; }
        public bool IsActive { get; set; }
    }
    public class PhotoDetailViewModel : AuthenticatedMasterPageViewModel
    {
        public PhotoService PhotoService { get; }
        public DirectoryService DirectoryService { get; }

        [FromRoute("Id")]
        public Guid PhotoId { get; set; }
        public List<CarouselPhotoItem> Photos { get; set; }
        public Photo OpenedInfo { get; set; }
        public bool EditMode { get; set; }
        public Guid ParentGuid => DirectoryService.GetParentGuid(PhotoId);
        public int ActivePhotoIndex { get; set; }
        public override Task Init()
        {
            Photos = PhotoService.GetAllPhotos(ParentGuid).Select(t => new CarouselPhotoItem()
            {
                Photo = t,
                IsActive = t.Id == PhotoId
            }).ToList();
            return base.Init();
        }

        public PhotoDetailViewModel(PhotoService photoService, DirectoryService directoryService)
        {
            PhotoService = photoService;
            DirectoryService = directoryService;
        }

        public void SaveDetails()
        {
            var updated = Photos.First(photo => photo.Photo.Id == OpenedInfo.Id);
            updated.Photo.Name = OpenedInfo.Name;
            updated.Photo.Description = OpenedInfo.Description;
            PhotoService.EditPhoto(updated.Photo);
            EditMode = false;
        }

        public void CancelChanges()
        {
            EditMode = false;
            var openedInfoId = OpenedInfo.Id;
            OpenedInfo = null;
            OpenedInfo = Photos.First(photo => photo.Photo.Id == openedInfoId).Photo;
        }
    }
}

