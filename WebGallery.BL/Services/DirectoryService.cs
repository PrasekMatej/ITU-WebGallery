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

        public void CreateDirectory(Folder Dir)
        {
            using (var context = new GalleryDbContext())
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
        public void CreateUserRootDirectory(IdentityUser user)
        {
            using (var context = new GalleryDbContext())
            {
                var directoryEntity = new DirectoryEntity
                {
                    Id = Guid.Parse(user.Id),
                    Parent = Guid.Empty,
                    Name = user.UserName,
                };

                context.Directories.Add(directoryEntity);
                context.SaveChanges();
            }

        }

        public void MovePhoto(Photo photo, Guid targetDirectory)
        {
            using (var context = new GalleryDbContext())
            {
                var foundPhoto = context.Photos.Find(photo.Id);
                foundPhoto.Parent = targetDirectory;
                context.Update(foundPhoto);
                context.SaveChanges();
            }
        }


        public void DeleteDirectory(Folder dir)
        {
            using (var context = new GalleryDbContext())
            {
                var toDelete = context.Directories.Find(dir.Id);
                if (toDelete == null)
                {
                    return;
                }
                DeleteAllPhotosInDirectory(toDelete.Id);

                context.Directories.Remove(toDelete);
                context.SaveChanges();
            }

        }

        public void DeleteAllPhotosInDirectory(Guid id)
        {
            using (var context = new GalleryDbContext())
            {
                context.Photos.RemoveRange(context.Photos.Where(t => t.Parent == id).ToArray());
                context.SaveChanges();
            }
        }

        public Folder GetDirectory(Guid id)
        {
            using (var context = new GalleryDbContext())
            {
                var directoryEntity = context.Directories.Find(id);
                if (directoryEntity == null)
                {
                    return null;
                }
                else
                {
                    return new Folder()
                    {
                        Parent = directoryEntity.Parent,
                        Name = directoryEntity.Name,
                        Id = directoryEntity.Id
                    };
                }
            }
        }
    }
}
