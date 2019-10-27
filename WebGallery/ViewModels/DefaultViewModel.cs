using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using DotVVM.Framework.Controls;
using WebGallery.BL.DTO;

namespace WebGallery.ViewModels
{
    public class DefaultViewModel : AuthenticatedMasterPageViewModel
    {
        public GridViewDataSet<string> FakeDataset { get; set; } = new GridViewDataSet<string>()
        {
            PagingOptions = new PagingOptions()
            {
                TotalItemsCount = 200,
                PageSize = 20
            }
        };

        public ICollection<PathModel> Path { get; set; }
        public Folder CurrentFolder { get; set; }
        public ICollection<Folder> Folders => CurrentFolderItems.Where(t => t is Folder).Cast<Folder>().ToList();
        public ICollection<Photo> Photos => CurrentFolderItems.Where(t => t is Photo).Cast<Photo>().ToList();

        protected ICollection<Item> CurrentFolderItems { get; set; } = new List<Item>();

        public int ColumnCount { get; set; } = 6;

        public DefaultViewModel()
        {
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

            Path = GetPath(CurrentFolder);
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
                folder = folder.Parent;
            }

            path.Reverse();
            return path;
        }
    }

    public class PathModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
