using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Talabat.APIs.Error;

namespace Talabat.APIs.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next ,ILogger<ExceptionMiddleware> logger,IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;//500 status code

                //var options = new JsonSerializerOptions()
                //{
                //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                //};

                var response = env.IsDevelopment() ?
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) 
                    :
                     new ApiExceptionResponse((int)HttpStatusCode.InternalServerError ,ex.Message);


                var options = new JsonSerializerOptions()//js name camalCase
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                var json = JsonSerializer.Serialize(response , options); ///تحويل من اوبجيكت الي جيسون
                //var json = JsonSerializer.Deserialize();//تحويل من جيسون الي اوبجيكت

               


                await context.Response.WriteAsync(json);


            }
        }
    }
}
