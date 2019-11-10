using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebGallery.BL.DAL;
using WebGallery.BL.DTO;

namespace WebGallery.BL.Services
{
    public class PhotoService
    {

        public void UploadPhoto(List<Photo> photos)
        {
            using (var context = new GalleryDbContext())
            {
                foreach (Photo photo in photos)
                {
                    var photoEntity = new PhotoEntity {
                        Id = Guid.NewGuid(),
                        Url = photo.Url,
                        Description = photo.Description,
                        Metadata = photo.Metadata,
                        CreatedDate = photo.CreatedDate,
                        Tags = photo.Tags.Select(tag => new TagEntity() {Name = tag.Name, Owner = tag.Owner, DeletedDate = tag.DeletedDate, Id = tag.Id   }).ToList(),
                    };
                    context.Photos.Add(photoEntity);
                }
                context.SaveChanges();
            }

        }

        public List<Photo> GetPhotos(Guid folderId, int pageNum, int pageSize)
        {
            using (var context = new GalleryDbContext() )
            {
                return context.Directories.Find(folderId).Items.OfType<PhotoEntity>().OrderByDescending(item => item.CreatedDate).Skip((pageNum - 1) * pageSize).Take(pageSize).Select(item => 
                new Photo() {
                    Id = item.Id, CreatedDate = item.CreatedDate, DeletedDate = item.DeletedDate, Description = item.Description, Metadata = item.Metadata, Name = item.Name, Tags = item.Tags.Select(tag => new Tag() { Name = tag.Name, Owner = tag.Owner, DeletedDate = tag.DeletedDate, Id = tag.Id }).ToList(), Url = item.Url }).ToList();
                
            }
        }

        public void EditPhoto(Photo photo)
        {
            using (var context = new GalleryDbContext())
            {
                var photoEntity = new PhotoEntity
                {
                    Url = photo.Url,
                    Description = photo.Description,
                    Metadata = photo.Metadata,
                    CreatedDate = photo.CreatedDate,
                    Tags = photo.Tags.Select(tag => new TagEntity() { Name = tag.Name, Owner = tag.Owner, DeletedDate = tag.DeletedDate, Id = tag.Id }).ToList(),
                };
                context.Photos.Update(photoEntity);
                context.SaveChanges();
            }                
        }

        public void DeletePhoto(PhotoEntity photo)
        {
            using (var context = _dbContext.CreateDbContext())
            {
                Console.WriteLine("Tu mala byt implementacia upload");
                return 1;
            }

        }

    }

}
