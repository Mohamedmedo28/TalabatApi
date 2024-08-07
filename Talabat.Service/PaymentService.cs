using Microsoft.Extensions.Configuration;
using Stripe;
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
using Product = Talabat.Core.Entities.Product; 

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBasketRepository basketRepository;
        private readonly IConfiguration configuration;

        public PaymentService(IUnitOfWork unitOfWork ,IBasketRepository basketRepository ,IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.basketRepository = basketRepository;
            this.configuration = configuration;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string BasketId)
        {
            StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];

            var basket =await basketRepository.GetBasketAsync(BasketId);

            if (basket == null) return null;
            var ShippingPrice = 0m;

            if(basket.DeliveryMethodId.HasValue)
            {
                var deliverMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                ShippingPrice = deliverMethod.Cost;
                basket.ShippingCost = deliverMethod.Cost; 
            }

            if(basket?.Items?.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await unitOfWork.Repository<Product>().GetByIdAsync(item.Id); 
                    if(item.Price != product.Price)
                        item.Price = product.Price; 
                }
            }

            var Service = new PaymentIntentService() ; 
            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))//create Payment Intent
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * item.Quantuty * 100) +(long) ShippingPrice * 100 ,
                    Currency ="usd",
                    PaymentMethodTypes  = new List<string>() { "card"}
                };

                paymentIntent = await Service.CreateAsync(options) ;

                basket.PaymentIntentId = paymentIntent.Id ;
                basket.ClientSecret = paymentIntent.ClientSecret; 

            }
            else //Update
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * item.Quantuty * 100) + (long)ShippingPrice * 100,
                   
                };
                await Service.UpdateAsync(basket.PaymentIntentId, options) ;
            }

            await basketRepository.UpdateBasketAsync(basket);
            return basket;
        }
    }
}
