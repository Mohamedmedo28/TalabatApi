using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregation;
using Talabat.Core.Repository;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPaymentService paymentService;

        public OrderService(IBasketRepository basketRepository ,
            
            IUnitOfWork unitOfWork , IPaymentService paymentService)
        {
            this.basketRepository = basketRepository;
            this.unitOfWork = unitOfWork;
            this.paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string BuyerEmail, string BasketId, int DeliveryMethodId, Address ShippingAddress)
        {
            // 1.Get Basket From Basket Repo
            var basket = await basketRepository.GetBasketAsync(BasketId);
            //2.Get Selected Items at Basket From Product Repo
                var orderItems = new List<OrderItem>();
            if(basket?.Items?.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                   // int productId, string productName, string pictureUrl)
                    var ProductItemOrder = new ProductOrderItem(product.Id , product.Name , product.PictureUrl);
                //    public OrderItem(ProductOrderItem product, decimal price, int quantity)
                    var orderItem = new OrderItem(ProductItemOrder , product.Price , item.Quantuty);

                    orderItems.Add(orderItem);  
                }
            }

            //3.Calculate SubTotal

            var SubTotal = orderItems.Sum(item => item.Price * item.Quantity);


            //4.Get Delivery Method From DeliveryMethod Repo

            var deliverMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);

            //// Check PaymentIntentId Exists From another Order  ((New ))
            var Spec = new OrderWithPaymentIntentSpecification(basket.PaymentIntentId);
            var ExOrder = await unitOfWork.Repository<Order>().GetWithSpecAsync(Spec);

            if(ExOrder != null)
            {
                unitOfWork.Repository<Order>().Delete(ExOrder);
                //update PaymentIntentId With Amount of basket if Changed
                basket = await paymentService.CreateOrUpdatePaymentIntent(BasketId); 
            }

            //5.Create Order

            ////

            var order = new Order(BuyerEmail, ShippingAddress, deliverMethod , orderItems ,SubTotal ,basket.PaymentIntentId );
            //6.Add Order Locally

            await unitOfWork.Repository<Order>().Add(order);

            //7.Save Order To Database[ToDo]
          var result =   await unitOfWork.Complete();

            if (result <= 0) return null; 

            return order;
           


        }

       
        public async Task<Order> GetOrderByIdForUserAsync(string BuyerEmail, int OrderId)
        {
            var spec = new OrderSpecification( OrderId , BuyerEmail);

            var Order = await unitOfWork.Repository<Order>().GetWithSpecAsync(spec);

            return Order; 

        }

        public async Task<IReadOnlyList<Order>> GetOrderForUserAsync(string BuyerEmail)
        {

            var spec = new OrderSpecification(BuyerEmail);
            var orders = await unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            return orders;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        {
            var DeliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

            return DeliveryMethod;
        }

    }
}
