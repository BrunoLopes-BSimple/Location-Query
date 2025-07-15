using System.Text;
using Newtonsoft.Json;

namespace InterfaceAdapters.Tests;

public class IntegrationTestBase
{
    protected readonly HttpClient Client;

    protected IntegrationTestBase(HttpClient client)
    {
        Client = client;
    }

    protected async Task<T> GetAndDeserializeAsync<T>(string url)
    {
        var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(body)!;
    }

    protected async Task<HttpResponseMessage> GetAsync(string url)
    {
        return await Client.GetAsync(url);
    }
}
