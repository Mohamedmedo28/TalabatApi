using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Error;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repository;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.APIs.Extentions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection addApplicationServices(this IServiceCollection services)
        {
            //
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork ,UnitOfWork>();
            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            services.AddScoped<IPaymentService , PaymentService>();

            services.AddScoped<IOrderService, OrderService>();

            //Auto MApper
            services.AddAutoMapper(typeof(MappingProfiles));
            //

            //Handel ValidationErrors 7
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                          .SelectMany(p => p.Value.Errors)
                                                           .Select(p => p.ErrorMessage).ToArray();

                    var validationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });

            //

            return services;
        }
    }
}
