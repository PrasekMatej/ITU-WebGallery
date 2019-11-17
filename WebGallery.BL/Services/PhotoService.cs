using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
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
        public void UploadPhoto(IEnumerable<Photo> photos)
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
                        Id = Guid.NewGuid(),
                        Parent = specDir.Id,
                        Url = photo.Url,
                        Description = photo.Description,
                        Metadata = photo.Metadata,
                        CreatedDate = photo.CreatedDate,
                        Tags = photo.Tags.Select(tag => new TagEntity() { Name = tag.Name, Owner = tag.Owner, Id = tag.Id }).ToList(),
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

                return context.Photos.Include(s => s.Tags).Where(p => p.Parent == folderId)
                    .OrderByDescending(item => item.CreatedDate)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize).Select(item =>
                new Photo()
                {
                    Id = item.Id,
                    CreatedDate = item.CreatedDate,
                    Description = item.Description,
                    Metadata = item.Metadata,
                    Name = item.Name,
                    Parent = item.Parent,
                    Tags = item.Tags.Select(tag => new Tag()
                    {
                        Name = tag.Name,
                        Owner = tag.Owner,
                        Id = tag.Id
                    }).ToList(),
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
                updatingPhoto.Tags = photo.Tags.Select(tag => new TagEntity() { Name = tag.Name, Owner = tag.Owner, Id = tag.Id }).ToList();
                context.Photos.Update(updatingPhoto);
                context.SaveChanges();
            }
        }

        public void DeletePhoto(Photo photo)
        {
            using (var context = dbContextFactory())
            {
                var toDelete = context.Photos.Find(photo.Id);

                context.Photos.Remove(toDelete);
                context.SaveChanges();
            }

        }

    }

    public class TagService
    {
        private readonly Func<GalleryDbContext> dbContextFactory;

        public TagService(Func<GalleryDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        public void CreateTags(ICollection<Tag> tags)
        {
            using (var context = dbContextFactory())
            {
                foreach (var tag in tags)
                {
                    if (tag.Id != Guid.Empty) continue;
                    var tagEntity = new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = tag.Name,
                        Owner = tag.Owner
                    };
                    context.Add(tagEntity);
                }

                context.SaveChanges();
            }
        }
    }

}
