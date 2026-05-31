using System.Net;
using System.Net.Http.Headers;

namespace ReportApp
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public ApiService(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var payload = new { Username = username, Password = password };

            string endpoint = _config["ApiSettings:LoginEndpoint"] ?? "api/AuthDummy/login";

            var response = await _client.PostAsJsonAsync(endpoint, payload); // placeholder

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<T>?> GetReportAsync<T>(string endpoint, string jwtToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _client.SendAsync(request);
            if(!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<T>>();
        }
    }
}
