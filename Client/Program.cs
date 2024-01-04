
    var accessToken = await RequestTokenAsync();

    if (!string.IsNullOrEmpty(accessToken))
    {
        await AccessProtectedResourceAsync(accessToken);
    }

Console.ReadLine();

static async Task<string> RequestTokenAsync()
{
    using var httpClient = new HttpClient();

    var tokenEndpoint = "http://localhost:5000/connect/token"; // Your IdentityServer token endpoint

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

        // Parse the token response and extract the access token
        // For simplicity, you may want to use a library like IdentityModel
        // Here, we are assuming a simple JSON response with an "access_token" property
        var accessToken = tokenResponseContent.Split('"')[3];
        return accessToken;
    }
    else
    {
        Console.WriteLine("Token Request Failed. Status Code: " + tokenResponse.StatusCode);
        return null;
    }
}

static async Task AccessProtectedResourceAsync(string accessToken)
{
    using var httpClient = new HttpClient();

    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

    var apiEndpoint = "http://localhost:5001/api/sample"; // Your protected API endpoint

    var apiResponse = await httpClient.GetAsync(apiEndpoint);

    if (apiResponse.IsSuccessStatusCode)
    {
        var apiResponseContent = await apiResponse.Content.ReadAsStringAsync();
        Console.WriteLine("API Response: " + apiResponseContent);
    }
    else
    {
        Console.WriteLine("API Request Failed. Status Code: " + apiResponse.StatusCode);
    }
}