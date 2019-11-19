using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotVVM.BusinessPack.Controls;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Storage;
using WebGallery.BL.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using WebGallery.BL.Services;

namespace WebGallery.ViewModels
{
    public class DefaultViewModel : AuthenticatedMasterPageViewModel
    {
        private readonly IUploadedFileStorage fileStorage;
        private readonly DirectoryService directoryService;
        private readonly PhotoService photoService;
        private readonly UserService _userService;
        private readonly IHostingEnvironment environment;
        public ICollection<Guid> SelectedPhotos { get; set; } = new List<Guid>();
        public DefaultViewModel(IUploadedFileStorage fileStorage, DirectoryService directoryService, PhotoService photoService, UserService userService, IHostingEnvironment environment)
        {
            this.fileStorage = fileStorage;
            this.directoryService = directoryService;
            this.photoService = photoService;
            _userService = userService;
            this.environment = environment;
        }

        public GridViewDataSet<Photo> PhotoDataset { get; set; } = new GridViewDataSet<Photo>()
        {
            PagingOptions = new PagingOptions()
            {
                PageSize = 20
            },
            IsRefreshRequired = true
        };

        public ICollection<PathModel> CurrentPath { get; set; }
        public Folder CurrentFolder { get; set; }
        public ICollection<Folder> Folders => CurrentFolderItems.Where(t => t is Folder).Cast<Folder>().ToList();

        protected ICollection<Item> CurrentFolderItems { get; set; } = new List<Item>();

        public int ColumnCount { get; set; } = 6;

        public bool IsUploadDialogVisible { get; set; }
        public bool IsDeleteDialogVisible { get; set; }
        public bool IsMoveDialogVisible { get; set; }
        public UploadData UploadData { get; set; } = new UploadData();
        public ICollection<Photo> UploadedPhotos { get; set; } = new List<Photo>();


        public override async Task Init()
        {

            var currentUser = await _userService.GetUser(Username);
            Guid directoryId = Guid.Parse(currentUser.Id);
            if (Context.Parameters.ContainsKey("Path"))
            {
                try
                {
                    directoryId = (Guid)Context.Parameters["Path"];
                }
                catch (Exception)
                {
                    // use default
                }
            }

            try
            {

                CurrentFolder = directoryService.GetDirectory(directoryId);
            }
            catch (Exception)
            {
                Context.RedirectToRoute("Default");
            }

            CurrentPath = GetPath(CurrentFolder);
            await base.Init();
        }

        public override Task PreRender()
        {
            if (PhotoDataset.IsRefreshRequired)
            {
                PhotoDataset.Items = photoService.GetPhotos(CurrentFolder.Id, PhotoDataset.PagingOptions.PageIndex, PhotoDataset.PagingOptions.PageSize).ToList();
                PhotoDataset.PagingOptions.TotalItemsCount = photoService.GetPhotoCount(CurrentFolder.Id);
                PhotoDataset.IsRefreshRequired = false;
            }
            return base.PreRender();
        }

        private ICollection<PathModel> GetPath(Folder currentFolder)
        {
            var path = new List<PathModel>();
            var folder = currentFolder;
            while (folder != null)
            {
                path.Add(new PathModel
                {
                    Name = folder.Name,
                    Id = folder.Id
                });
                folder = directoryService.GetDirectory(folder.Parent);
            }

            path.Reverse();
            return path;
        }

        public void Save()
        {
            var folderPath = GetFolderPath();
            var uploadedPhotos = UploadedPhotos;


            // save all files to disk
            foreach (var file in UploadData.Files)
            {
                var filePath = Path.Combine(folderPath, file.FileId + Path.GetExtension(file.FileName));
                fileStorage.SaveAs(file.FileId, filePath);
                fileStorage.DeleteFile(file.FileId);
                uploadedPhotos.Single(t => t.Id == file.FileId).Url =
                    $"/UserPhotos/{file.FileId + Path.GetExtension(file.FileName)}";
            }
            photoService.UploadPhotos(uploadedPhotos);

            // clear the data so the user can continue with other files
            UploadData.Clear();
            IsUploadDialogVisible = false;
        }

        private string GetFolderPath()
        {
            var folderPath = Path.Combine(environment.WebRootPath, "UserPhotos");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        public void DeleteUploadedFile(Guid id)
        {
            UploadData.Files.Remove(UploadData.Files.Find(file => file.FileId == id));
            UploadedPhotos.Remove(UploadedPhotos.First(file => file.Id == id));
        }

        public void UploadCompleted()
        {

            foreach (var uploadDataFile in UploadData.Files)
            {
                if (!UploadedPhotos.Any(t => t.Id == uploadDataFile.FileId))
                {
                    UploadedPhotos.Add(new Photo()
                    {
                        Id = uploadDataFile.FileId,
                        CreatedDate = DateTime.Now,
                        Name = Path.GetFileNameWithoutExtension(uploadDataFile.FileName),
                        Url = uploadDataFile.PreviewUrl,
                        Parent = CurrentFolder.Id
                    });
                }
            }
        }

        public ICollection<Folder> CurrentlyBrowsedDirectories { get; set; }

        public void GoToFolder(Guid destination)
        {
            //TODO
        }
        public void MovePhotos(Guid destination)
        {
            //TODO
        }

        public void DeletePhotos()
        {
            //TODO
        }

        public void OpenMoveModalDialog()
        {
            IsMoveDialogVisible = true;
        }

        public void ReloadData()
        {
            PhotoDataset.RequestRefresh();
        }
    }

    public class PathModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
