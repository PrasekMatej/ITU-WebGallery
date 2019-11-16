using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebGallery.BL.DAL;
using WebGallery.BL.DTO;

namespace WebGallery.BL.Services
{
    class DirectoryService
    {

        public void CreateDirectory(Folder Dir)
        {
            using (var context = new GalleryDbContext())
            {
                var directoryEntity = new DirectoryEntity
                {
                    Id = Guid.NewGuid(),
                    Parent = Dir.Parent.Id,
                    Name = Dir.Name,
                };

                context.Directories.Add(directoryEntity);
                context.SaveChanges();
            }

        }


        public void DeleteDirectory(Folder dir)
        {
            using (var context = new GalleryDbContext())
            {
                var toDelete = context.Directories.Find(dir.Id);

                //also delete pictures in it

                context.Directories.Remove(toDelete);
                context.SaveChanges();
            }

        }


    }
}
