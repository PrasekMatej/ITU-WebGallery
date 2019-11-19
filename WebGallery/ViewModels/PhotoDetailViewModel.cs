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
    public class PhotoDetailViewModel : AuthenticatedMasterPageViewModel
    {
        public PhotoService PhotoService { get; }
        public DirectoryService DirectoryService { get; }

        [FromRoute("Id")]
        public Guid PhotoId { get; set; }
        public List<Photo> Photos { get; set; }
        public Photo OpenedInfo { get; set; }
        public bool EditMode { get; set; }
        public override Task Init()
        {
            var folderGuid = DirectoryService.GetParentGuid(PhotoId);
            Photos = PhotoService.GetAllPhotos(folderGuid);
            return base.Init();
        }

        public PhotoDetailViewModel(PhotoService photoService, DirectoryService directoryService)
        {
            PhotoService = photoService;
            DirectoryService = directoryService;
        }

        public void SaveDetails()
        {
            var updated = Photos.First(photo => photo.Id == OpenedInfo.Id);
            updated.Name = OpenedInfo.Name;
            updated.Description = OpenedInfo.Description;
            EditMode = false;
        }

        public void CancelChanges()
        {
            EditMode = false;
            var openedInfoId = OpenedInfo.Id;
            OpenedInfo = null;
            OpenedInfo = Photos.First(photo => photo.Id == openedInfoId);
        }

        public void PrevPhoto()
        {
            //throw new NotImplementedException();
        }

        public void NextPhoto()
        {
           // throw new NotImplementedException();
        }
    }
}

