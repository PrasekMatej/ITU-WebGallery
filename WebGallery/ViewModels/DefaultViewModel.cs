using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using DotVVM.BusinessPack.Controls;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Storage;
using WebGallery.BL.DTO;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel;
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
        public ICollection<Folder> Folders { get; set; }


        public int ColumnCount { get; set; } = 6;

        public bool IsUploadDialogVisible { get; set; }
        public bool IsDeleteDialogVisible { get; set; }
        public bool IsMoveDialogVisible { get; set; }
        public UploadData UploadData { get; set; } = new UploadData();
        public ICollection<Photo> UploadedPhotos { get; set; } = new List<Photo>();

        public override async Task Load()
        {
            if (!Context.IsPostBack)
            {
                var currentUser = await _userService.GetUser(Username);
                Guid directoryId = Guid.Parse(currentUser.Id);
                if (Context.Parameters.ContainsKey("id"))
                {
                    try
                    {
                        var contextParameter = Guid.Parse(Context.Parameters["id"].ToString());
                        if (contextParameter != Guid.Empty)
                        {
                            directoryId = contextParameter;
                        }
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
            }

            await base.Load();
        }

        public override Task PreRender()
        {
            if (PhotoDataset.IsRefreshRequired)
            {
                PhotoDataset.Items = photoService.GetPhotos(CurrentFolder.Id, PhotoDataset.PagingOptions.PageIndex, PhotoDataset.PagingOptions.PageSize).ToList();
                PhotoDataset.PagingOptions.TotalItemsCount = photoService.GetPhotoCount(CurrentFolder.Id);
                PhotoDataset.IsRefreshRequired = false;

                Folders = directoryService.GetChildDirectories(CurrentFolder.Id);
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
            CurrentFolder = directoryService.GetDirectory(destination);
            ReloadData();
        }
        public void MovePhotos(Guid destination)
        {
            directoryService.MovePhotos(SelectedPhotos, destination);
            SelectedPhotos.Clear();
            IsMoveDialogVisible = false;
            DirStructure = null;
            ReloadData();
        }

        public void DeletePhotos()
        {
            photoService.DeletePhotos(SelectedPhotos);
            SelectedPhotos.Clear();
            IsDeleteDialogVisible = false;
            ReloadData();
        }

        public async Task OpenMoveModalDialog()
        {

            Guid userId = new Guid((await _userService.GetUser(Username)).Id);
            DirStructure = new List<FolderHierarchy>() { directoryService.GetDirectoryStructure(userId, CurrentFolder.Id) };
            IsMoveDialogVisible = true;
        }

        public ICollection<FolderHierarchy> DirStructure { get; set; }

        public void ReloadData()
        {
            PhotoDataset.RequestRefresh();
        }

        public bool IsCreateDirectoryModalDisplayed { get; set; }
        public string NewDirectoryName { get; set; }

        public void CreateDirectory()
        {
            IsCreateDirectoryModalDisplayed = false;
            directoryService.CreateDirectory(new Folder()
            {
                Parent = CurrentFolder.Id,
                Name = NewDirectoryName
            });
            NewDirectoryName = null;
            ReloadData();
        }
    }

    public class PathModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
