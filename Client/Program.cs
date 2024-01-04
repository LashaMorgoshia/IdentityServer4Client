class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var accessToken = await RequestTokenAsync();

            if (!string.IsNullOrEmpty(accessToken))
            {
                await AccessProtectedResourceAsync(accessToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task<string> RequestTokenAsync()
    {
        try
        {
            using var httpClient = new HttpClient();

            var tokenEndpoint = "https://localhost:7074/connect/token"; // Your IdentityServer token endpoint

            var clientId = "client";
            var clientSecret = "secret";
            var scope = "api1";

            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", scope)
            });

            var tokenResponse = await httpClient.PostAsync(tokenEndpoint, tokenRequest);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Token Response: " + tokenResponseContent);

                var accessToken = tokenResponseContent.Split('"')[3];
                return accessToken;
            }
            else
            {
                var errorResponse = await tokenResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Token Request Failed. Status Code: " + tokenResponse.StatusCode);
                Console.WriteLine("Error Response: " + errorResponse);
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token Request Exception: {ex.Message}");
            return null;
        }
    }

    static async Task AccessProtectedResourceAsync(string accessToken)
    {
        try
        {
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var apiEndpoint = "https://localhost:7074/api/sample"; // Your protected API endpoint

            var apiResponse = await httpClient.GetAsync(apiEndpoint);

            if (apiResponse.IsSuccessStatusCode)
            {
                var apiResponseContent = await apiResponse.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + apiResponseContent);
            }
            else
            {
                var errorResponse = await apiResponse.Content.ReadAsStringAsync();
                Console.WriteLine("API Request Failed. Status Code: " + apiResponse.StatusCode);
                Console.WriteLine("Error Response: " + errorResponse);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API Request Exception: {ex.Message}");
        }
    }
}