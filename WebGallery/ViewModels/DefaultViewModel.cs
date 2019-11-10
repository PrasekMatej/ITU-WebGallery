using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using DotVVM.BusinessPack.Controls;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Storage;
using WebGallery.BL.DTO;

namespace WebGallery.ViewModels
{
    public class DefaultViewModel : AuthenticatedMasterPageViewModel
    {
        private readonly IUploadedFileStorage fileStorage;

        public DefaultViewModel(IUploadedFileStorage fileStorage)
        {
            this.fileStorage = fileStorage;

            CurrentFolder = new Folder
            {
                Id = Guid.NewGuid(),
                Name = "Some other folder",
                Parent = new Folder()
                {
                    Id = Guid.NewGuid(),
                    Name = "Some folder",
                    Parent = new Folder()
                    {
                        Id = Guid.NewGuid(),
                        Name = ".."
                    }
                }
            };

            var random = new Random();
            for (int i = 0; i < 50; i++)
            {
                var height = random.Next(100, 300);
                var width = random.Next(100, 300);
                CurrentFolderItems.Add(new Photo
                {
                    Parent = CurrentFolder,
                    CreatedDate = DateTime.Now.Subtract(TimeSpan.FromDays(i)),
                    Name = "Photo name",
                    Description = $"Photo description {i}",
                    Url = $@"https://picsum.photos/id/{i}/{width}/{height}",
                    Height = height,
                    Width = width
                });
            }


            CurrentFolderItems.Add(new Folder()
            {
                Name = "Folder 1",
                Parent = CurrentFolder,
                Id = Guid.NewGuid()
            });

            CurrentFolderItems.Add(new Folder()
            {
                Name = "Folder 2",
                Parent = CurrentFolder,
                Id = Guid.NewGuid()
            });
            CurrentFolderItems.Add(new Folder()
            {
                Name = "Folder 3",
                Parent = CurrentFolder,
                Id = Guid.NewGuid()
            });

            CurrentPath = GetPath(CurrentFolder);
        }

        public GridViewDataSet<string> FakeDataset { get; set; } = new GridViewDataSet<string>()
        {
            PagingOptions = new PagingOptions()
            {
                TotalItemsCount = 200,
                PageSize = 20
            }
        };

        public ICollection<PathModel> CurrentPath { get; set; }
        public Folder CurrentFolder { get; set; }
        public ICollection<Folder> Folders => CurrentFolderItems.Where(t => t is Folder).Cast<Folder>().ToList();
        public ICollection<Photo> Photos => CurrentFolderItems.Where(t => t is Photo).Cast<Photo>().ToList();

        protected ICollection<Item> CurrentFolderItems { get; set; } = new List<Item>();

        public int ColumnCount { get; set; } = 6;

        public bool IsUploadDialogVisible { get; set; }
        public UploadData UploadData { get; set; } = new UploadData();
        public ICollection<Photo> UploadedPhotos { get; set; } = new List<Photo>();
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
                folder = folder.Parent;
            }

            path.Reverse();
            return path;
        }

        public void Save()
        {
            var folderPath = GetFolderPath();

            // save all files to disk
            foreach (var file in UploadData.Files)
            {
                var filePath = Path.Combine(folderPath, file.FileId + Path.GetExtension(file.FileName));
                fileStorage.SaveAs(file.FileId, filePath);
                fileStorage.DeleteFile(file.FileId);
            }

            // clear the data so the user can continue with other files
            UploadData.Clear();
        }

        private string GetFolderPath()
        {
            var folderPath = Path.Combine(Context.Configuration.ApplicationPhysicalPath, "UserPhotos");

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
                        Url = uploadDataFile.PreviewUrl
                    });
                }
            }
        }
    }

    public class PathModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
