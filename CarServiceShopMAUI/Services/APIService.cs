// csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CarServiceShopMAUI.Models;

namespace CarServiceShopMAUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Parameterless fallback to preserve existing usage
        public ApiService() : this(new HttpClient { BaseAddress = new Uri("https://your-api-url.com/api/") })
        {
        }

        // Preferred ctor for DI (IHttpClientFactory)
        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("https://your-api-url.com/api/");
            }
        }

        private void LogError(string context, Exception ex)
        {
            Debug.WriteLine($"ApiService error ({context}): {ex}");
        }

        // Car CRUD
        public async Task<List<Car>> GetCarsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Car>>("car", _jsonOptions);
                return result ?? new List<Car>();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetCarsAsync), ex);
                return new List<Car>();
            }
        }

        public async Task<Car?> GetCarByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Car>($"car/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                LogError($"{nameof(GetCarByIdAsync)}({id})", ex);
                return null;
            }
        }

        public async Task<bool> AddCarAsync(Car newCar)
        {
            if (newCar == null) throw new ArgumentNullException(nameof(newCar));
            try
            {
                var response = await _httpClient.PostAsJsonAsync("car", newCar, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError(nameof(AddCarAsync), ex);
                return false;
            }
        }

        public async Task<bool> UpdateCarAsync(Car updatedCar)
        {
            if (updatedCar == null) throw new ArgumentNullException(nameof(updatedCar));
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"car/{updatedCar.Id}", updatedCar, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError(nameof(UpdateCarAsync), ex);
                return false;
            }
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"car/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError($"{nameof(DeleteCarAsync)}({id})", ex);
                return false;
            }
        }

        public async Task<List<Service>> GetServicesForCarAsync(int carId)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Service>>($"car/{carId}/services", _jsonOptions);
                return result ?? new List<Service>();
            }
            catch (Exception ex)
            {
                LogError($"{nameof(GetServicesForCarAsync)}({carId})", ex);
                return new List<Service>();
            }
        }

        public async Task<List<Part>> GetPartsForServiceAsync(int serviceId)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Part>>($"service/{serviceId}/parts", _jsonOptions);
                return result ?? new List<Part>();
            }
            catch (Exception ex)
            {
                LogError($"{nameof(GetPartsForServiceAsync)}({serviceId})", ex);
                return new List<Part>();
            }
        }

        // Service CRUD
        public async Task<List<Service>> GetServicesAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Service>>("service", _jsonOptions);
                return result ?? new List<Service>();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetServicesAsync), ex);
                return new List<Service>();
            }
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Service>($"service/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                LogError($"{nameof(GetServiceByIdAsync)}({id})", ex);
                return null;
            }
        }

        public async Task<bool> AddServiceAsync(Service newService)
        {
            if (newService == null) throw new ArgumentNullException(nameof(newService));
            try
            {
                var response = await _httpClient.PostAsJsonAsync("service", newService, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError(nameof(AddServiceAsync), ex);
                return false;
            }
        }

        public async Task<bool> UpdateServiceAsync(Service updatedService)
        {
            if (updatedService == null) throw new ArgumentNullException(nameof(updatedService));
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"service/{updatedService.Id}", updatedService, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError(nameof(UpdateServiceAsync), ex);
                return false;
            }
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"service/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError($"{nameof(DeleteServiceAsync)}({id})", ex);
                return false;
            }
        }

        // Part CRUD
        public async Task<List<Part>> GetPartsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Part>>("part", _jsonOptions);
                return result ?? new List<Part>();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetPartsAsync), ex);
                return new List<Part>();
            }
        }

        public async Task<Part?> GetPartByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Part>($"part/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                LogError($"{nameof(GetPartByIdAsync)}({id})", ex);
                return null;
            }
        }

        public async Task<bool> AddPartAsync(Part newPart)
        {
            if (newPart == null) throw new ArgumentNullException(nameof(newPart));
            try
            {
                var response = await _httpClient.PostAsJsonAsync("part", newPart, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError(nameof(AddPartAsync), ex);
                return false;
            }
        }

        public async Task<bool> UpdatePartAsync(Part updatedPart)
        {
            if (updatedPart == null) throw new ArgumentNullException(nameof(updatedPart));
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"part/{updatedPart.Id}", updatedPart, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError(nameof(UpdatePartAsync), ex);
                return false;
            }
        }

        public async Task<bool> DeletePartAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"part/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError($"{nameof(DeletePartAsync)}({id})", ex);
                return false;
            }
        }
    }
}
