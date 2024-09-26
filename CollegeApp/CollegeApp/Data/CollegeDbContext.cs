using CollegeApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CollegeApp.Data
{
    public class CollegeDbContext : DbContext
    {
        public CollegeDbContext(DbContextOptions<CollegeDbContext> options) : base(options)
        {
            
        }

        public DbSet<Student> Students { get; set; }
    }
}
