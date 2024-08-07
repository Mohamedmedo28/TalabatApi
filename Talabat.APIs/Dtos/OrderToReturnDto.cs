using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.APIs.Dtos
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }

        public DateTimeOffset OrderDate { get; set; } 

        public OrderStatus Status { get; set; } 

        public Address ShippingAddress { get; set; }

        //  public int DeliveryMethodId { get; set; }

        public string DeliveryMethod { get; set; }
        public decimal DeliveryMethodCost { get; set; }

        public ICollection<OrderItemDto> Items { get; set; } = new HashSet<OrderItemDto>();

        public decimal SubTotal { get; set; }
        //[NotMapped]
        //public decimal Total => SubTotal + DeliveryMethod.Cost ;
        public decimal Total { get; set; }

        public string PaymentIntentId { get; set; }
    }
}
