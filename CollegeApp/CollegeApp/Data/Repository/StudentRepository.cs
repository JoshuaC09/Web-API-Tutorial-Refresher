using CollegeApp.Model;
using Microsoft.EntityFrameworkCore;

namespace CollegeApp.Data.Repository
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        private readonly CollegeDbContext _context;
        public StudentRepository(CollegeDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetStudentsByFeeStatusAsync(int feeStatus)
        {
            return await _context.Students.ToListAsync();
        }
    }
}
