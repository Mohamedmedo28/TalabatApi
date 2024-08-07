using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using Talabat.APIs.Dtos;
using Talabat.APIs.Error;
using Talabat.Core.Entities;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
   
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentsController> logger;
        private const string endpointSecret = "whsec_e456a494ac85f17836a35437feeeca5c0e5b0e5043afac36e734b7e8b165e5fe";
        public PaymentsController(IPaymentService paymentService ,ILogger<PaymentsController> logger)
        {
            this.paymentService = paymentService;
            this.logger = logger;
        }
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentService(string basketId)
        {
            var basket = await paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) return BadRequest(new ApiErrorResponse(400, "A problem With Your Basket")); 

            return Ok(basket);
        }

        [AllowAnonymous]
        [HttpPost("{webhook}")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
           
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await paymentService.UpdatePaymentIntentSuccessedOrFailed(PaymentIntent.Id, false);
                    logger.LogInformation("Payment is Successed " , PaymentIntent.Id);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await paymentService.UpdatePaymentIntentSuccessedOrFailed(PaymentIntent.Id, true);
                    logger.LogInformation("Payment is Failed :(", PaymentIntent.Id);


                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
