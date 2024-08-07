using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.Core.Specifications.Order_Spec
{
    public class OrderSpecification :BaseSpecifications<Order>
    {
        public OrderSpecification(string email):base(o=>o.BuyerEmail == email)
        {
            Includes.Add(o=>o.DeliveryMethod);
            Includes.Add(o=>o.Items);

            addOrderByDescending(o => o.OrderDate); 
        }

        public OrderSpecification( int id ,string email ) 
            : base(o => o.BuyerEmail == email && o.Id ==id) 
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);

        }
    }
}
