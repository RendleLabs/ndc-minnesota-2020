using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Core;
using Toppings.Data;

namespace Toppings
{
    public class ToppingsService : Toppings.ToppingsBase
    {
        private static readonly DiagnosticListener Diagnostics =
            new DiagnosticListener("Toppings");
        
        private static readonly string ServiceId = Guid.NewGuid().ToString("d");
        private readonly IToppingData _data;

        public ToppingsService(IToppingData data)
        {
            _data = data;
        }

        public override async Task<AvailableResponse> GetAvailable(AvailableRequest request, Grpc.Core.ServerCallContext context)
        {
            List<ToppingEntity> toppings;

            try
            {
                toppings = await _data.GetAsync();
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

            var headers = new Metadata
            {
                new Metadata.Entry("service-id", ServiceId)
            };

            await context.WriteResponseHeadersAsync(headers);

            var response = new AvailableResponse();
            foreach (var entity in toppings)
            {
                var topping = new Topping
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Price = entity.Price
                };
                var availableTopping = new AvailableTopping
                {
                    Topping = topping,
                    Quantity = entity.StockCount
                };
                response.Toppings.Add(availableTopping);
            }

            return response;
        }

        public override async Task<DecrementStockResponse> DecrementStock(DecrementStockRequest request, ServerCallContext context)
        {
            Activity activity = null;

            if (Diagnostics.IsEnabled())
            {
                activity = new Activity("DecrementStockInAzure");
                var ids = string.Join(',', request.ToppingIds);
                activity.AddTag("topping_ids", ids);
                
                Diagnostics.StartActivity(activity, null);
            }
            foreach (var id in request.ToppingIds)
            {
                await _data.DecrementStockAsync(id);
            }

            if (!(activity is null))
            {
                Diagnostics.StopActivity(activity, null);
            }

            return new DecrementStockResponse();
        }
    }
}