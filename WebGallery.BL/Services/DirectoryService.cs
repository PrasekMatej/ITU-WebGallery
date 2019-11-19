using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using WebGallery.BL.DAL;
using WebGallery.BL.DTO;

namespace WebGallery.BL.Services
{
    public class DirectoryService
    {
        private readonly Func<GalleryDbContext> contextFactory;

        public DirectoryService(Func<GalleryDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }
        public void CreateDirectory(Folder Dir)
        {
            using (var context = contextFactory())
            {
                var directoryEntity = new DirectoryEntity
                {
                    Id = Guid.NewGuid(),
                    Parent = Dir.Parent,
                    Name = Dir.Name,
                };

                context.Directories.Add(directoryEntity);
                context.SaveChanges();
            }

        }

        public void MovePhotos(IEnumerable<Guid> photoIds, Guid newDirectory)
        {
            using (var context = contextFactory())
            {
                foreach (var photoId in photoIds)
                {
                    var photoEntity = context.Photos.Find(photoId);
                    photoEntity.Parent = newDirectory;
                }

                context.SaveChanges();
            }
        }


        public void DeleteDirectory(Folder dir)
        {
            using (var context = contextFactory())
            {
                var toDelete = context.Directories.Find(dir.Id);
                if (toDelete == null)
                {
                    return;
                }

                var photos = context.Photos.Where(t => t.Parent == dir.Id);
                context.Photos.RemoveRange(photos);

                context.Directories.Remove(toDelete);
                context.SaveChanges();
            }

        }


        public Folder GetDirectory(Guid directory)
        {
            using (var context = contextFactory())
            {
                var directoryEntity = context.Directories.Find(directory);
                if (directoryEntity == null)
                {
                    return null;
                }

                return new Folder()
                {
                    Parent = directoryEntity.Parent,
                    Name = directoryEntity.Name,
                    Id = directoryEntity.Id
                };
            }
        }
        public void CreateUserRootDirectory(IdentityUser user)
        {
            using (var context = contextFactory())
            {
                var directoryEntity = new DirectoryEntity
                {
                    Id = new Guid(user.Id),
                    Parent = Guid.Empty,
                    Name = user.UserName,
                };

                context.Directories.Add(directoryEntity);
                context.SaveChanges();
            }

        }

        public FolderHierarchy GetDirectoryStructure(Guid rootId, Guid currentId)
        {
            var dict = new Dictionary<Guid, FolderHierarchy>();
            using (var context = contextFactory())
            {
                var root = createFolderHierarchy(context.Directories.Find(rootId), currentId);
                var toExplore = new Queue<DirectoryEntity>(context.Directories.Where(t => t.Parent == rootId).ToList());
                dict.Add(rootId, root);

                while (toExplore.Count != 0)
                {
                    var dir = toExplore.Dequeue();
                    var currentFolder = dict[dir.Parent];
                    var folderHierarchy = createFolderHierarchy(dir, currentId);
                    dict.Add(dir.Id, folderHierarchy);
                    currentFolder.ChildFolders.Add(folderHierarchy);

                    var childDirs = context.Directories.Where(t => t.Parent == dir.Id).ToList();
                    foreach (var childDir in childDirs)
                    {
                        toExplore.Enqueue(childDir);
                    }
                }

                return root;
            }
        }

        protected FolderHierarchy createFolderHierarchy(DirectoryEntity dir, Guid currentId)
        {
            return new FolderHierarchy()
            {
                IsCurrent = dir.Id == currentId,
                Parent = dir.Parent,
                Name = dir.Name,
                Id = dir.Id,
                ChildFolders = new List<FolderHierarchy>()
            };
        }

        public IList<Folder> GetChildDirectories(Guid id)
        {
            using (var context = contextFactory())
            {
                return context.Directories.Where(t => t.Parent == id).Select(t => new Folder()
                {
                    Name = t.Name,
                    Parent = t.Parent,
                    Id = t.Id
                }).ToList();
            }
        }
    }

    public class FolderHierarchy : Folder
    {
        public bool IsCurrent { get; set; }
        public ICollection<FolderHierarchy> ChildFolders { get; set; }
    }
}
