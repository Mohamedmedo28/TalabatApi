using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    internal static class SpecificationsEvaluator<Tentity> where Tentity : BaseEntity
    {
        public static IQueryable<Tentity> GetQuery(IQueryable<Tentity> InputQuery , ISpecifications<Tentity> spec)
        {
            var query = InputQuery;//context.set<Product>()

            if(spec.Criteria != null)//p=>p.Id == 1
                query = query.Where(spec.Criteria);

            if(spec.OrderBy is not null)//p=>p.Name
                query = query.OrderBy(spec.OrderBy);


            if (spec.OrderByDescending is not null)//p=>p.price
                query = query.OrderByDescending(spec.OrderByDescending);

            //query = context.set<Product>().where(p=>p.id == 1).OrderBy(p=>p.Name).OrderByDes(p=>p.price)

            if (spec.IsPaginationEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            //query = context.set<Product>().where(p=>p.id == 1)
            //include
            //1-p=>p.brand
            //2-p=>p.category
            query = spec.Includes.Aggregate(query, (CurrentQuery, includeExpression) =>
                      CurrentQuery.Include(includeExpression) 
            );

            //context.set<Product>().where(p=>p.id == 1).include(p=>p.brand)
            //context.set<Product>().where(p=>p.id == 1).include(p=>p.brand).include(p=>p.category)

            return query;
        }
    }
}
