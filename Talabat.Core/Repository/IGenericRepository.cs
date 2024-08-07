using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repository
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);

        Task<T?> GetWithSpecAsync(ISpecifications<T> Spec);

        Task<int> GetCountWithSpecAsync(ISpecifications<T> Spec);

        Task Add(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
