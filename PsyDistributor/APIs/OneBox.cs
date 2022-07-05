using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PsyDistributor.APIs;

internal class Auth
{
    public Auth()
    {
        login = "restapi";
        restapipassword = "425c6273b33c540a85626593f15ddf0d";
    }
    public string login { get; set; }
    public string restapipassword { get; set; }

}
internal class Token
{
    public dataArray dataArray { get; set; }
    public string status { get; set; }
}
internal class dataArray
{
    public string token { get; set; }
}

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
}

class OneBox
{
    static HttpClient client = new HttpClient();

    internal static async void OneBoxInit()
    {
        await RunAsync();
        Console.WriteLine("OneBox module initialized");
    }

    static void ShowProduct(Product product)
    {
        Console.WriteLine($"Name: {product.Name}\tPrice: " +
            $"{product.Price}\tCategory: {product.Category}");
    }

    static async Task<Uri> CreateProductAsync(Product product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "api/products", product);
        response.EnsureSuccessStatusCode();

        // return URI of the created resource.
        return response.Headers.Location;
    }

    static async Task<Product> GetProductAsync(string path)
    {
        Product product = null;
        HttpResponseMessage response = await client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            product = await response.Content.ReadAsAsync<Product>();
        }
        return product;
    }

    static async Task<Product> UpdateProductAsync(Product product)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"api/products/{product.Id}", product);
        response.EnsureSuccessStatusCode();

        // Deserialize the updated product from the response body.
        product = await response.Content.ReadAsAsync<Product>();
        return product;
    }

    static async Task<HttpStatusCode> DeleteProductAsync(string id)
    {
        HttpResponseMessage response = await client.DeleteAsync(
            $"api/products/{id}");
        return response.StatusCode;
    }

    internal static async Task GetToken(string path)
    {
        var auth = new Auth();
        var token = new Token();
        HttpResponseMessage response = await client.PostAsJsonAsync($"{path}/api/v2/token/get/", auth);
        if (response.IsSuccessStatusCode)
        {
            token = await response.Content.ReadAsAsync<Token>();
        }
        Console.WriteLine($"{response.StatusCode}\n{token.dataArray.token}");
    }



    static async Task RunAsync()
    {
        // Update port # in the following line.
        client.BaseAddress = new Uri("https://testingapi.1b.app");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        await GetToken($"{client.BaseAddress}");

        try
        {
            // Create a new product
            Product product = new Product
            {
                Name = "Gizmo",
                Price = 100,
                Category = "Widgets"
            };

            var url = await CreateProductAsync(product);
            Console.WriteLine($"Created at {url}");

            // Get the product
            product = await GetProductAsync(url.PathAndQuery);
            ShowProduct(product);

            // Update the product
            Console.WriteLine("Updating price...");
            product.Price = 80;
            await UpdateProductAsync(product);

            // Get the updated product
            product = await GetProductAsync(url.PathAndQuery);
            ShowProduct(product);

            // Delete the product
            var statusCode = await DeleteProductAsync(product.Id);
            Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadLine();
    }
}