using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microservice.Web.Utility;

namespace Microservice.Web.Services
{
    public class BaseService : IBaseService
    {
        public async Task<ResultDto<T>> SendAsync<T>(RequestDto requestDto)
        {
            try
            {
                using var client = new HttpClient();

                if (!string.IsNullOrEmpty(requestDto.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestDto.AccessToken);
                }

                HttpResponseMessage response = requestDto.APIType switch
                {
                    ApiType.GET => await client.GetAsync(requestDto.URL),
                    ApiType.POST => await client.PostAsync(requestDto.URL, new StringContent(JsonSerializer.Serialize(requestDto.Data), Encoding.UTF8, "application/json")),
                    ApiType.PUT => await client.PutAsync(requestDto.URL, new StringContent(JsonSerializer.Serialize(requestDto.Data), Encoding.UTF8, "application/json")),
                    ApiType.DELETE => await client.DeleteAsync(requestDto.URL),
                    _ => throw new InvalidOperationException("Unsupported API type")
                };

                var content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = JsonSerializer.Deserialize<ResultDto<T>>(content, options);
                        if (result != null)
                            return result;

                        var data = content is T t ? t : default(T)!;
                        return ResultDto<T>.SuccessResult(data, "Request succeeded but response could not be deserialized.");
                    }
                    catch
                    {
                        var data = content is T t ? t : default(T)!;

                        return ResultDto<T>.SuccessResult(data, "Request succeeded but response could not be deserialized.");
                    }
                }

                // Non-success status code
                return ResultDto<T>.FailureResult("Something went wrong please try again");
            }
            catch (Exception ex)
            {
                return ResultDto<T>.FailureResult(ex.Message);
            }
        }
    }
}
