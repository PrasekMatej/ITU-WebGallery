using System;
using System.Collections.Generic;
using System.Linq;
using WebGallery.BL.DAL;
using WebGallery.BL.DTO;

namespace WebGallery.BL.Services
{
    public class PhotoService
    {
        private readonly Func<GalleryDbContext> dbContextFactory;

        public PhotoService(Func<GalleryDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        public void UploadPhotos(IEnumerable<Photo> photos)
        {
            using (var context = dbContextFactory())
            {
                foreach (Photo photo in photos)
                {
                    var specDir = context.Directories.Find(photo.Parent);
                    if (specDir == null)
                    {
                        throw new ArgumentException("Directory does not exist!");
                    }

                    var photoEntity = new PhotoEntity
                    {
                        Name = photo.Name,
                        Id = photo.Id == Guid.Empty ? Guid.NewGuid() : photo.Id,
                        Parent = specDir.Id,
                        Url = photo.Url,
                        Description = photo.Description,
                        Metadata = photo.Metadata,
                        CreatedDate = photo.CreatedDate,
                    };
                    context.Photos.Add(photoEntity);

                }
                context.SaveChanges();
            }

        }

        public ICollection<Photo> GetPhotos(Guid folderId, int pageNum, int pageSize)
        {
            using (var context = dbContextFactory())
            {

                return context.Photos.Where(p => p.Parent == folderId)
                    .OrderByDescending(item => item.CreatedDate)
                    .Skip(pageNum * pageSize)
                    .Take(pageSize).Select(item =>
                new Photo()
                {
                    Id = item.Id,
                    CreatedDate = item.CreatedDate,
                    Description = item.Description,
                    Metadata = item.Metadata,
                    Name = item.Name,
                    Parent = item.Parent,
                    Url = item.Url

                }).ToList();
            }
        }

        public List<Photo> GetAllPhotos(Guid folderId)
        {
            using (var context = dbContextFactory())
            {

                return context.Photos.Where(p => p.Parent == folderId)
                    .OrderByDescending(item => item.CreatedDate)
                    .Select(item =>
                new Photo()
                {
                    Id = item.Id,
                    CreatedDate = item.CreatedDate,
                    Description = item.Description,
                    Metadata = item.Metadata,
                    Name = item.Name,
                    Parent = item.Parent,
                    Url = item.Url

                }).ToList();
            }
        }

        public void EditPhoto(Photo photo)
        {

            using (var context = dbContextFactory())
            {

                var updatingPhoto = context.Photos.Find(photo.Id);
                updatingPhoto.Metadata = photo.Metadata;
                updatingPhoto.Name = photo.Name;
                updatingPhoto.Description = photo.Description;
                updatingPhoto.Url = photo.Url;
                updatingPhoto.Parent = photo.Parent;
                context.Photos.Update(updatingPhoto);
                context.SaveChanges();
            }
        }

        public void DeletePhotos(IEnumerable<Guid> photoIds)
        {
            using (var context = dbContextFactory())
            {
                var photosToDelete = new List<PhotoEntity>();
                foreach (var photoId in photoIds)
                {
                    photosToDelete.Add(context.Photos.Find(photoId));
                }
                context.Photos.RemoveRange(photosToDelete);
                context.SaveChanges();
            }
        }

        public int GetPhotoCount(Guid FolderId)
        {
            using (var context = dbContextFactory())
            {
                return context.Photos.Count(t => t.Parent == FolderId);
            }
        }
    }
}
