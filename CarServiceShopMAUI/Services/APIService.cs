using System.Net.Http.Json;
using CarServiceShopMAUI.Models;

namespace CarServiceShopMAUI.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://your-api-url.com/api/")
        };
    }

    // Car CRUD
    public async Task<List<Car>> GetCarsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Car>>("car");
    }

    public async Task<Car> GetCarByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Car>($"car/{id}");
    }

    public async Task<bool> AddCarAsync(Car newCar)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("car", newCar);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCarAsync(Car updatedCar)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"car/{updatedCar.Id}", updatedCar);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCarAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"car/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Service>> GetServicesForCarAsync(int carId)
    {
        return await _httpClient.GetFromJsonAsync<List<Service>>($"car/{carId}/services");
    }

    public async Task<List<Part>> GetPartsForServiceAsync(int serviceId)
    {
        return await _httpClient.GetFromJsonAsync<List<Part>>($"service/{serviceId}/parts");
    }

    // Service CRUD
    public async Task<List<Service>> GetServicesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Service>>("service");
    }

    public async Task<Service> GetServiceByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Service>($"service/{id}");
    }

    public async Task<bool> AddServiceAsync(Service newService)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("service", newService);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateServiceAsync(Service updatedService)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"service/{updatedService.Id}", updatedService);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"service/{id}");
        return response.IsSuccessStatusCode;
    }

    // Part CRUD
    public async Task<List<Part>> GetPartsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Part>>("part");
    }

    public async Task<Part> GetPartByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Part>($"part/{id}");
    }

    public async Task<bool> AddPartAsync(Part newPart)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("part", newPart);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdatePartAsync(Part updatedPart)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"part/{updatedPart.Id}", updatedPart);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeletePartAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"part/{id}");
        return response.IsSuccessStatusCode;
    }
}