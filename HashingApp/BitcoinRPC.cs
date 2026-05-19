using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HashingApp
{
    public class BitcoinRPC
    {
        private readonly HttpClient client;

        public BitcoinRPC(string url, string user, string pass)
        {
            client = new HttpClient { BaseAddress = new Uri(url) };
            var ba = Encoding.ASCII.GetBytes($"{user}:{pass}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ba));
        }

        public JsonElement Call(string method, object[] pars = null)
        {
            var payload = new
            {
                jsonrpc = "1.0",
                id = "miner",
                method = method,
                @params = pars ?? new object[] { }
            };

            string json = JsonSerializer.Serialize(payload);
            var res = client.PostAsync("", new StringContent(json, Encoding.UTF8, "application/json")).Result;
            string resStr = res.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(resStr);
            return doc.RootElement.GetProperty("result");
        }
    }
}
