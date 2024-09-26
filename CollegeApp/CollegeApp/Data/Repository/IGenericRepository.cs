
using CollegeApp.Model;
using System.Linq.Expressions;

namespace CollegeApp.Data.Repository
{
    public interface IGenericRepository <T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Expression<Func<T,bool>> filter, bool useNoTracking = false);

        Task<T> CreateAsync(T dbRecord);
        Task<T> UpdateAsync(T dbRecord);

        Task<bool> DeleteAsync(T dbRecord);
    }
}
