using System;
using System.Threading.Tasks;
using Xunit;

namespace Toppings.Tests
{
    public class ToppingsTests : IClassFixture<ToppingsApplicationFactory>
    {
        private readonly ToppingsApplicationFactory _factory;

        public ToppingsTests(ToppingsApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetsList()
        {
            var client = _factory.CreateToppingsClient();

            var response = await client.GetAvailableAsync(new AvailableRequest());
            Assert.Equal(2, response.Toppings.Count);
        }
    }
}
