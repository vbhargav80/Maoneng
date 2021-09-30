using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApplication.Domain
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(string id);
        Task Save(T item);
        Task Remove(string id);
    }
}
