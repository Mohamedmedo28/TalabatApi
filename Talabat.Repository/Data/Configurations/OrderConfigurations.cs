using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.Repository.Data.Configurations
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(s => s.Status)
                   .HasConversion(
                    OS => OS.ToString(),
                    OS => (OrderStatus)Enum.Parse(typeof(OrderStatus), OS));

            builder.OwnsOne(ad => ad.ShippingAddress, SA => SA.WithOwner());

            builder.HasOne(d => d.DeliveryMethod)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);

            builder.Property(p => p.SubTotal)
                    .HasColumnType("decimal(18,2)");

            builder.HasMany(o => o.Items)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
