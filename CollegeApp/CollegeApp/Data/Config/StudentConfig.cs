using CollegeApp.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CollegeApp.Data.Config
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(n => n.Name).IsRequired();
            builder.Property(n => n.Name).HasMaxLength(250);
            builder.Property(n => n.Email).IsRequired().HasMaxLength(250);

            builder.HasData(new List<Student>()
            {
                new Student {
                    Id = 1,
                    Name = "Venkat",
                    Email="venkat@gmail.com",
                    Address="Santa Maria",
                    DOB = new DateTime(2022,12,12)
                },
                new Student {
                    Id = 2,
                    Name = "Nehanth",
                    Email="nehanth@gmail.com",
                    Address="Makati",
                    DOB = new DateTime(2022,06,12)
                }
            });

            builder.HasOne(n => n.Department)
                .WithMany(n => n.Students)
                .HasForeignKey(n=>n.DepartmentId)
                .HasConstraintName("FK_Students_Department");
        }
    }
}
