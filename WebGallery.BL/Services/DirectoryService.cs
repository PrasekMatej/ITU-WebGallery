using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using WebGallery.BL.DAL;
using WebGallery.BL.DTO;

namespace WebGallery.BL.Services
{
    public class DirectoryService
    {
        private readonly Func<GalleryDbContext> contextFactory;
        private readonly PhotoService photoService;

        public DirectoryService(Func<GalleryDbContext> contextFactory, PhotoService photoService)
        {
            this.contextFactory = contextFactory;
            this.photoService = photoService;
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

        public void MovePhoto(Photo photo, Guid newDirectory)
        {
            photo.Parent = newDirectory;
            photoService.EditPhoto(photo);
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

        public Guid GetParentGuid(Guid id)
        {
            using (var context = contextFactory())
            {
                return context.Photos.Find(id).Parent;
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
    }
}
