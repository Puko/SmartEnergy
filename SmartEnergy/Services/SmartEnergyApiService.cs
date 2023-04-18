using Newtonsoft.Json;
using SmartEnergy.Api;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Database.Models;
using SmartEnergy.Interfaces;
using SmartEnergy.Models;
using System;
using System.Text;

namespace SmartEnergy.Services
{
    public class SmartEnergyApiService
    { 
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://backend.merito.tech/") };
        private readonly ILogService _logService;

        public SmartEnergyApiService(ILogService logService)
        {
            _logService = logService;
        }

        public async Task<ApiResult<UserInformation>> LoginAsync(string login, string password)
        {
            var credentials = JsonConvert.SerializeObject(new
            {
                login,
                password
            });

            try
            {
                var response = await _httpClient.PostAsync("api2-r/auth/local", new StringContent(credentials, Encoding.UTF8, "application/json"));
                var info = await ProcessResponse<UserInformation>(response);
                if (info.Succes)
                {
                    var data = info.Value.Devices.Where(x => !x.Owner).ToList();
                    foreach (var item in data)
                    {
                        info.Value.Devices.Remove(item);
                    }
                }
                return info;
            }
            catch(Exception ex)
            {
                _logService.Exception(ex, "Login request failed.");
                return new ApiResult<UserInformation>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public Task<ApiResult<SetRelayResponse>> SetRelay1Async(string token, bool enabled)
        {
            var set = ValueToJson(new
            {
                token,
                relay_1 = enabled
            });

            return SetRelayAsync(set);
        } 
        
        public Task<ApiResult<SetRelayResponse>> SetRelay1NameAsync(string token, string name)
        {
            var set = ValueToJson(new
            {
                token,
                names = new { R1 = name, }
            });

            return SetRelayNameAsync(set);
        }
        
        public Task<ApiResult<SetRelayResponse>> SetRelay2Async(string token, bool enabled)
        {
            var set = ValueToJson(new
            {
                token,
                relay_2 = enabled
            });

            return SetRelayAsync(set);
        }

        public Task<ApiResult<SetRelayResponse>> SetRelay2NameAsync(string token, string name)
        {
            var set = ValueToJson(new
            {
                token,
                names = new { R2 = name, }
            });

            return SetRelayNameAsync(set);
        }

        public Task<ApiResult<SetRelayResponse>> SetRelay3Async(string token, bool enabled)
        {
            var set = ValueToJson(new
            {
                token,
                relay_3 = enabled
            });

            return SetRelayAsync(set);
        }

        public Task<ApiResult<SetRelayResponse>> SetRelay3NameAsync(string token, string name)
        {
            var set = ValueToJson(new
            {
                token,
                names = new { R3 = name, }
            });

            return SetRelayNameAsync(set);
        }

        public Task<ApiResult<SetRelayResponse>> SetRelay4Async(string token, bool enabled)
        {
            var set = ValueToJson(new
            {
                token,
                relay_4 = enabled
            });

            return SetRelayAsync(set);
        }

        public Task<ApiResult<SetRelayResponse>> SetRelay4NameAsync(string token, string name)
        {
            var set = ValueToJson(new
            {
                token,
                names = new { R4 = name, }
            });

            return SetRelayNameAsync(set);
        }

        public async Task<ApiResult<RelaySettingsResponse>> GetDeviceRelaySettings(string token)
        {
            var set = ValueToJson(new
            {
                token,
                names = true,
                power = true
            });


            var response = await _httpClient.PostAsync("api2-r/disconnector/settings", GetContent(set));
            return await ProcessResponse<RelaySettingsResponse>(response);
        }

        private Task<ApiResult<SetRelayResponse>> SetRelayAsync(string content)
        {
            return EditRelayAsync("api2-w/disconnector/set-relay", GetContent(content));
        }
        
        private Task<ApiResult<SetRelayResponse>> SetRelayNameAsync(string content)
        {
            return EditRelayAsync("api2-w/disconnector/settings", GetContent(content));
        }

        public async Task<ApiResult<SetRelayResponse>> EditRelayAsync(string url, StringContent content)
        {
            try
            {
                var response = await _httpClient.PostAsync(url, content);
                return await ProcessResponse<SetRelayResponse>(response);
            }
            catch (Exception ex)
            {
                _logService.Exception(ex, "Set relay request failed.");
                return new ApiResult<SetRelayResponse>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        private async Task<ApiResult<T>> ProcessResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            ApiResult<T> result;
            if (response.IsSuccessStatusCode)
            {
                var responseValue = JsonToValue<T>(content);
                result = new ApiResult<T>(responseValue, response.StatusCode);
            }
            else
            {
                result = new ApiResult<T>(content, response.StatusCode);
            }

            return result;
        }

        private TValue JsonToValue<TValue>(string json)
        {
            TValue result = JsonConvert.DeserializeObject<TValue>(json);
            return result;
        }

        private static StringContent GetContent(string value)
        {
            StringContent content = new StringContent(value, Encoding.UTF8, "application/json");
            return content;
        }

        private static string ValueToJson(object value)
        {
            string serializedValue = JsonConvert.SerializeObject(value);
            return serializedValue;
        }
    }
}
