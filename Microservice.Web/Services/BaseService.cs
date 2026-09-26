using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microservice.Web.Utility;

namespace Microservice.Web.Services
{
    public class BaseService(ITokenProvider tokenProvider) : IBaseService
    {
        public async Task<ResultDto<T>> SendAsync<T>(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                using var client = new HttpClient();

                if (withBearer)
                {
                    var token = tokenProvider.GetToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                else if (!string.IsNullOrEmpty(requestDto.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestDto.AccessToken);
                }
                if (requestDto.ContentType == ContentType.MultipartFormData)
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
                }
                else
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                HttpResponseMessage response = null;

                if (requestDto.ContentType == ContentType.MultipartFormData)
                {
                    var contents = new MultipartFormDataContent();
                    foreach (var prop in requestDto.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(requestDto.Data);
                        if (value is not null)
                        {
                            if (value is IFormFile file)
                            {
                                var fileContent = new StreamContent(file.OpenReadStream());
                                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                                contents.Add(fileContent, prop.Name, file.FileName);

                            }
                            else
                            {
                                contents.Add(new StringContent(value.ToString() ?? string.Empty), prop.Name);
                            }

                        }
                    }
                    if (requestDto.APIType == ApiType.POST)
                    {
                        response = await client.PostAsync(requestDto.URL, contents);
                    }
                    else if (requestDto.APIType == ApiType.PUT)
                    {
                        response = await client.PutAsync(requestDto.URL, contents);
                    }
                }
                else
                {
                    response = requestDto.APIType switch
                    {
                        ApiType.GET => await client.GetAsync(requestDto.URL),
                        ApiType.POST => await client.PostAsync(requestDto.URL, new StringContent(JsonSerializer.Serialize(requestDto.Data), Encoding.UTF8, "application/json")),
                        ApiType.PUT => await client.PutAsync(requestDto.URL, new StringContent(JsonSerializer.Serialize(requestDto.Data), Encoding.UTF8, "application/json")),
                        ApiType.DELETE => await client.DeleteAsync(requestDto.URL),
                        _ => throw new InvalidOperationException("Unsupported API type")
                    };
                }


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
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return ResultDto<T>.FailureResult(content);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return ResultDto<T>.FailureResult(content);
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
