using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repository;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext context;

        public GenericRepository(StoreContext context)
        {
            this.context = context;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            //if(typeof(T) == typeof(Product))
            //    return (IEnumerable<T>) await context.Products.Include(p=>p.ProductType).Include(p=>p.ProductBrand).ToListAsync(); 
          return await context.Set<T>().ToListAsync();
        }
        public async Task<T> GetByIdAsync(int id)
        {
            //if (typeof(T) == typeof(Product))
            //    return await context.Products.Where(p => p.Id == id).Include(p => p.ProductType).Include(p => p.ProductBrand).FirstOrDefaultAsync() as T;
           return await context.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpecifications(Spec).ToListAsync();
        }
        public async Task<T?> GetWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpecifications(Spec).FirstOrDefaultAsync();
        }
        public async Task<int> GetCountWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpecifications(Spec).CountAsync();
        }
        private IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(context.Set<T>(), spec); 
        }

        public async Task Add(T entity)
        {
           await context.Set<T>().AddAsync(entity);

        }

        public void Update(T entity)
        {
             context.Set<T>().Update(entity);

        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);

        }
    }
}
