using System.Net.Http;
using System.Threading.Tasks;

namespace Frontend
{
    public interface IAuthHelper
    {
        ValueTask<string> GetTokenAsync();
    }

    public class AuthHelper : IAuthHelper
    {
        private readonly HttpClient _client;
        private string _token;

        public AuthHelper(HttpClient client)
        {
            _client = client;
        }

        public ValueTask<string> GetTokenAsync()
        {
            return _token is null
                ? new ValueTask<string>(GetTokenAsyncImpl())
                : new ValueTask<string>(_token);
        }

        private async Task<string> GetTokenAsyncImpl()
        {
            _token = await _client.GetStringAsync("/generateJwtToken?name=frontend");
            return _token;
        }
    }
}