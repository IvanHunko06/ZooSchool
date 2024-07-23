
using Client.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
namespace Client.Services;

public class NetworkService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration configuration;
    private readonly ILogger<NetworkService> logger;
    private readonly HttpClientHandler _httpClientHandler;
    private readonly CookieContainer cookieContainer;
    private readonly Uri domain;

    public NetworkService(IConfiguration configuration, ILogger<NetworkService> logger)
    {
        domain = new Uri(configuration["Server:Domain"] ?? "");
        cookieContainer = new CookieContainer();
        _httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
        _httpClient = new HttpClient(_httpClientHandler);
        this.configuration = configuration;
        this.logger = logger;
        _httpClient.BaseAddress = domain;
    }
    public void DeleteTokenCookie()
    {
        var cookies = cookieContainer.GetCookies(domain);
        foreach (Cookie cookie in cookies)
        {
            if (cookie.Name == (configuration["Server:TokenCookie"]??""))
            {
                cookie.Expired = true;
            }
        }
    }

    public async Task<TResponseBodyModel?> GetRequest<TResponseBodyModel>(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponseBodyModel>(responseBody);
        }
        catch
        {
            return default;
        }

    }
    public async Task<string> GetRequest(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return "";
        }

    }

    public async Task<int> PostRequest(string url)
    {
        try
        {
            var response = await _httpClient.PostAsync(url, null);
            return (int)response.StatusCode;
        }
        catch (Exception ex)
        {
            return default;
        }
    }
    public async Task<TResponseBody?> PostRequest<TRequestBody, TResponseBody>(string url, TRequestBody body)
    {
        try
        {
            string bodyJson = JsonSerializer.Serialize(body);
            StringContent content = new(bodyJson, encoding: Encoding.UTF8, "text/json");
            var response = await _httpClient.PostAsync(url, content);
            return JsonSerializer.Deserialize<TResponseBody>(await response.Content.ReadAsStringAsync());
        }
        catch(Exception ex) 
        {
            return default;
        }

    }
    public async Task<TResponseBody?> PatchRequest<TRequestBody, TResponseBody>(string url, TRequestBody body)
    {
        try
        {
            string bodyJson = JsonSerializer.Serialize(body);
            StringContent content = new(bodyJson, encoding: Encoding.UTF8, "text/json");
            var response = await _httpClient.PatchAsync(url, content);
            return JsonSerializer.Deserialize<TResponseBody>(await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return default;
        }
    }
    public async Task<int> DeleteRequest(string url)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(url);
            return (int)response.StatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return default;
        }
    }
}
