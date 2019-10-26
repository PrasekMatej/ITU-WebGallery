using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebGallery.BL.DAL
{
    public class GalleryDbContext : IdentityDbContext
    {
        public DbSet<PhotoEntity> Photos { get; set; }
        public DbSet<DirectoryEntity> Directories { get; set; }

        public GalleryDbContext()
        {

        }
        public GalleryDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
