using System.Threading.Tasks;

namespace FantasyGaming.Infrastructure.Repository
{
    public interface IRepository<T>
    {
        Task AddAsync(T entity);
    }
}
