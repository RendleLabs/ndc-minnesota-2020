using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Orders
{
    public class OrdersService : Orders.OrdersBase
    {
        private readonly Toppings.ToppingsClient _toppingsClient;

        public OrdersService(Toppings.ToppingsClient toppingsClient)
        {
            _toppingsClient = toppingsClient;
        }

        public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, Grpc.Core.ServerCallContext context)
        {
            var decrementRequest = new DecrementStockRequest();
            decrementRequest.ToppingIds.AddRange(request.ToppingIds);
            await _toppingsClient.DecrementStockAsync(decrementRequest);

            return new PlaceOrderResponse
            {
                Time = DateTimeOffset.UtcNow.ToTimestamp()
            };
        }
    }
}
