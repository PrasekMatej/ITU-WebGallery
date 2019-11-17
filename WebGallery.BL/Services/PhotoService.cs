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

        public void UploadPhoto(IEnumerable<Photo> photos) //, Directory belongingDirecotry
        {
            using (var context = new GalleryDbContext())
            {
                foreach (Photo photo in photos)
                {
                    var specDir = context.Directories.Find(photo.Parent.Id);
                    if (specDir == null)
                    {
                        throw new ArgumentException("Directory does not exist!");
                    }

                    var photoEntity = new PhotoEntity {
                        Id = Guid.NewGuid(),
                        Parent = specDir,
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

                var photos = context.Photos.Where(p => p.Parent.Id == folderId)
                    .OrderByDescending(item => item.CreatedDate)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize);

                var result = new List<Photo>();

                foreach (var item in photos)
                {
                    var filledPhoto = new Photo()
                    {
                        Id = item.Id,
                        CreatedDate = item.CreatedDate,
                        DeletedDate = item.DeletedDate,
                        Description = item.Description,
                        Metadata = item.Metadata,
                        Name = item.Name,

                        Tags = item.Tags.Select(tag => new Tag()
                        {
                            Name = tag.Name,
                            Owner = tag.Owner,
                            DeletedDate = tag.DeletedDate,
                            Id = tag.Id
                        }).ToList(),
                        Url = item.Url

                    };

                    while (folder != null)
                    {
                        path.Add(new PathModel
                        {
                            Name = folder.Name,
                            Id = folder.Id
                        });
                        folder = folder.Parent;
                    }
                }

                    .Select(item => 
                new Photo() {
                    Id = item.Id, 
                    CreatedDate = item.CreatedDate, 
                    DeletedDate = item.DeletedDate, 
                    Description = item.Description, 
                    Metadata = item.Metadata, 
                    Name = item.Name, 
                    
                    Tags = item.Tags.Select(tag => new Tag() { 
                        Name = tag.Name, 
                        Owner = tag.Owner, 
                        DeletedDate = tag.DeletedDate, 
                        Id = tag.Id 
                    }).ToList(), 
                    Url = item.Url 
                    
                }).ToList();
                foreach (var item in photos)
                {
                    var folder = item.Parent;
                    while (folder != null)
                    {
                        path.Add(new PathModel
                        {
                            Name = folder.Name,
                            Id = folder.Id
                        });
                        folder = folder.Parent;
                    }
                }
            }
        }

        public void EditPhoto(Photo photo)
        {
            using (var context = new GalleryDbContext())
            {
                
                var updatingPhoto = context.Photos.Find(photo.Id);

                updatingPhoto.Metadata = photo.Metadata;
                updatingPhoto.Name = photo.Name;

                ...
                ..


                context.Photos.Update(updatingPhoto);
                context.SaveChanges();
            }                
        }

        public void DeletePhoto(Photo photo)
        {
            using (var context = new GalleryDbContext())
            {
                var toDelete = context.Photos.Find(photo.Id);

                context.Photos.Remove(toDelete);
                context.SaveChanges();
            }

        }

    }

}
