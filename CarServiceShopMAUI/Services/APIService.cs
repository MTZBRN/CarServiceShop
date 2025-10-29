// csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ApiService() : this(new HttpClient { BaseAddress = new Uri("http://localhost:5083/api/") }) { }

        // Preferred ctor for DI (IHttpClientFactory)
        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("http://localhost:5083/api/");
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
                Debug.WriteLine("🔄 Calling API: GET car");
                var result = await _httpClient.GetFromJsonAsync<List<Car>>("car", _jsonOptions);
                Debug.WriteLine($"✅ API Success: Received {result?.Count ?? 0} cars");
                return result ?? new List<Car>();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetCarsAsync), ex);
                Debug.WriteLine($"❌ API Error: {ex.Message}");
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

        public async Task<List<Service>> GetServicesForCarAsync(int carId)
        {
            try
            {
                Debug.WriteLine($"🔄 Calling API: GET service/bycar/{carId}");
                var result = await _httpClient.GetFromJsonAsync<List<Service>>($"service/bycar/{carId}", _jsonOptions);
                Debug.WriteLine($"✅ API Success: Received {result?.Count ?? 0} services for car {carId}");
                return result ?? new List<Service>();
            }
            catch (Exception ex)
            {
                LogError($"{nameof(GetServicesForCarAsync)}({carId})", ex);
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
                // DEBUG: Nézd meg a teljes JSON-t
                var json = System.Text.Json.JsonSerializer.Serialize(newService, _jsonOptions);
                Debug.WriteLine($"📤 JSON being sent: {json}");

                var response = await _httpClient.PostAsJsonAsync("service", newService, _jsonOptions);

                Debug.WriteLine($"📥 Response: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Error body: {errorBody}");
                }

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

        /// <summary>
        /// 🔧 Javított metódus - most már a helyes endpoint-ot használja!
        /// </summary>
        public async Task<List<Part>> GetPartsForServiceAsync(int serviceId)
        {
            try
            {
                Debug.WriteLine($"🔄 Calling API: GET part/byservice/{serviceId}");
                // Ez most már a helyes endpoint: part/byservice/{serviceId}
                var result = await _httpClient.GetFromJsonAsync<List<Part>>($"part/byservice/{serviceId}", _jsonOptions);
                Debug.WriteLine($"✅ API Success: Received {result?.Count ?? 0} parts for service {serviceId}");
                return result ?? new List<Part>();
            }
            catch (Exception ex)
            {
                LogError($"{nameof(GetPartsForServiceAsync)}({serviceId})", ex);
                Debug.WriteLine($"❌ Error getting parts for service {serviceId}: {ex.Message}");
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
                Debug.WriteLine("=== PART ADD DEBUG START ===");
                Debug.WriteLine($"🔍 ServiceId: {newPart.ServiceId}");
                Debug.WriteLine($"🔍 PartNumber: {newPart.PartNumber}");
                Debug.WriteLine($"🔍 Name: {newPart.Name}");
                Debug.WriteLine($"🔍 Price: {newPart.Price}");
                Debug.WriteLine($"🔍 Quantity: {newPart.Quantity}");
                Debug.WriteLine($"🔍 Description: {newPart.Description}");
                Debug.WriteLine($"🔍 Base URL: {_httpClient.BaseAddress}");

                // JSON serialization teszt
                var json = System.Text.Json.JsonSerializer.Serialize(newPart, _jsonOptions);
                Debug.WriteLine($"📤 JSON being sent: {json}");

                var response = await _httpClient.PostAsJsonAsync("part", newPart, _jsonOptions);

                Debug.WriteLine($"📥 Response Status: {response.StatusCode}");
                Debug.WriteLine($"📥 Response IsSuccess: {response.IsSuccessStatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Error Response Body: {errorBody}");
                }
                else
                {
                    var successBody = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"✅ Success Response Body: {successBody}");
                }

                Debug.WriteLine("=== PART ADD DEBUG END ===");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in AddPartAsync: {ex.Message}");
                Debug.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
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