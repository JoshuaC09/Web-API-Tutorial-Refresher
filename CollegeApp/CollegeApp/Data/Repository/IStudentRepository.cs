using CollegeApp.Model;

namespace CollegeApp.Data.Repository
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
         Task<IEnumerable<Student>> GetStudentsByFeeStatusAsync (int feeStatus);
    }
}
