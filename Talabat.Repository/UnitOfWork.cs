using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repository;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext context;

        private Hashtable _Repository;

        public UnitOfWork(StoreContext context)
        {
            this.context = context;
            _Repository = new Hashtable();

        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name;//product, order

            if(!_Repository.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity>(context);
                _Repository.Add(type, repository);
            }

            return _Repository[type] as IGenericRepository<TEntity>; 


        }

        public async Task<int> CompleteAsync()
            =>await context.SaveChangesAsync();
        

        public async ValueTask DisposeAsync()
       => await context.DisposeAsync();
        

   
    }
}
