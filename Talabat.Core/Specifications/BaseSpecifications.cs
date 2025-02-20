﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; } = null;
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>> ();
        public Expression<Func<T, object>> OrderBy { get; set ; }
        public Expression<Func<T, object>> OrderByDescending { get; set ; }
        public int Take { get ; set ; }
        public int Skip { get ; set ; }
        public bool IsPaginationEnabled { get; set ; }

        public BaseSpecifications()
        {
            //Criteria == null
        }

        public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;//p=>p.id == 10
        }

        public void addOrderBy(Expression<Func<T, object>> orderBy)
        {
            OrderBy = orderBy;
        }

        public void addOrderByDescending(Expression<Func<T, object>> orderByDescending)
        {
           OrderByDescending = orderByDescending;
        }

        public void ApplyPagination(int skip,int take)
        {
            IsPaginationEnabled = true;
            Skip = skip;
            Take = take;

        }
    }
}
