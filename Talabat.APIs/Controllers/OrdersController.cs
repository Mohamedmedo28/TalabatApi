using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Error;
using Talabat.Core.Entities.Order_Aggregation;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class OrdersController : ApiBaseController
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            //CreateMap<AddressDto, Talabat.Core.Entities.Order_Aggregation.Address>();

            var address = mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);

            var Order = await orderService.CreateOrderAsync(BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethod, address);

            if (Order == null) return BadRequest(new ApiErrorResponse(400));

            return Ok(Order);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var order = await orderService.GetOrderForUserAsync(BuyerEmail);

            var mappedOrders = mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(order);

            return Ok(mappedOrders);
        }
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderForUser(int id)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var Order = await orderService.GetOrderByIdForUserAsync(BuyerEmail ,id);

            if (Order == null) return BadRequest(new ApiErrorResponse(404));

            var mappedOrders = mapper.Map<Order, OrderToReturnDto>(Order);


            return Ok(mappedOrders);
        }

        [HttpGet("DeliveryMethod")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethod = await orderService.GetDeliveryMethodAsync();

            return Ok(deliveryMethod);  
        }
    }
}
