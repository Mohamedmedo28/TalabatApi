using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Error;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    
    public class BuggyController : ApiBaseController
    {
        private readonly StoreContext context;

        public BuggyController(StoreContext context)
        {
            this.context = context;
        }
        //1
        [HttpGet("NotFound")]//api/Buggy/NotFound   //code 404
        public ActionResult GetNotFoundRequest()
        {
            var products = context.Products.Find(100);
            if(products is null) return NotFound(new ApiErrorResponse(404));

            return Ok(products);
        }
        //2
        [HttpGet("ServerError")] //api/Buggy/ServerError
        public ActionResult GetServerErrorRequest()
        {
            var products = context.Products.Find(100);
            var ProductToReturn = products.ToString();

            return Ok(ProductToReturn);
        }
        //3
        [HttpGet("BadRequest")] //api/Buggy/BadRequest
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiErrorResponse(400));
        }
        //4
        [HttpGet("BadRequest/{id}")]  //api/Buggy/BadRequest/5
        public ActionResult GetBadRequest(int id)
        {
          return BadRequest();
        }
        //5
        //Donte show
    }
}
