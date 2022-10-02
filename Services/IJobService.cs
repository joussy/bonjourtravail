using bonjourtravail_api.Models;
using System.Linq.Expressions;

namespace bonjourtravail_api.Services
{
    public interface IJobService
    {
        Task DeleteAsync(string id);
        Task DeleteManyAsync(Expression<Func<Job, bool>> filter);
        Task<List<Job>> GetAsync();
        Task<Job?> GetAsync(string id);
        Task InsertAsync(Job newJob);
        Task InsertManyAsync(IEnumerable<Job> newJob);
    }
}