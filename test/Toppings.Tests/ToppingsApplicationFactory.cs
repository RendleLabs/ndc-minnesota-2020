using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Toppings.Tests
{
    public class ToppingsApplicationFactory : WebApplicationFactory<Startup>
    {
        public Toppings.ToppingsClient CreateToppingsClient()
        {
            var client = CreateDefaultClient(new ResponseVersionHandler());

            var channel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });

            return new Toppings.ToppingsClient(channel);
        }

        private class ResponseVersionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;
                return response;
            }
        }
    }
}
