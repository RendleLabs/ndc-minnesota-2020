using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Toppings.Data;
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
            var list = new List<ToppingEntity>
            {
                new ToppingEntity("cheese", "Cheese", 0.5m, 1),
                new ToppingEntity("tomato", "Tomato", 0.5m, 1),
            };

            var sub = Substitute.For<IToppingData>();
            sub.GetAsync().Returns(list);
            _factory.MockToppingData = sub;

            var client = _factory.CreateToppingsClient();

            var response = await client.GetAvailableAsync(new AvailableRequest());
            Assert.Equal(2, response.Toppings.Count);
        }

        [Fact]
        public async Task DecrementsStock()
        {
            var sub = Substitute.For<IToppingData>();
            _factory.MockToppingData = sub;

            var client = _factory.CreateToppingsClient();

            await client.DecrementStockAsync(new DecrementStockRequest {
                ToppingIds = { "cheese", "tomato" }
            });

            await sub
                .Received(1)
                .DecrementStockAsync("cheese", Arg.Any<CancellationToken>());

            await sub
                .Received(1)
                .DecrementStockAsync("tomato", Arg.Any<CancellationToken>());
        }
    }
}
