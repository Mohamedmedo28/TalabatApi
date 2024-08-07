using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Error;

namespace Talabat.APIs.Controllers
{
    [Route("errors/{code}")] 
    [ApiController]
    [ApiExplorerSettings (IgnoreApi =true)]
    public class ErrorsController : ControllerBase
    {
        public ActionResult Errors(int code)
        {
            return NotFound(new ApiErrorResponse(code));
        }
    }
}
