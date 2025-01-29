using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using MinimoShared;
using Newtonsoft.Json;

public class GameClient
{
    public AccountDTO AccountInfo { get; set; }

    private readonly string _baseUrl;

    public GameClient(string baseUrl)
    {
        _baseUrl = baseUrl;
    }
    
    
    // private class BypassCertificateHandler : CertificateHandler
    // {
    //     protected override bool ValidateCertificate(byte[] certificateData)
    //     {
    //         return true; // 모든 인증서 허용 (배포용에선 제거)
    //     }
    // }


    private void AddAuthorizationHeader(UnityWebRequest request)
    {
        var jwtToken = App.GetManager<LoginManager>().JwtToken;
        if (!string.IsNullOrEmpty(jwtToken))
        {
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
        }
    }

    private async UniTask<UnityWebRequest> SendRequestAsync(UnityWebRequest request)
    {
        AddAuthorizationHeader(request);
        await request.SendWebRequest().ToUniTask();

        // 요청 실패 시 상태 코드와 메시지를 포함한 예외 던지기
        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            var statusCode = request.responseCode; // 서버 응답 상태 코드 (0이면 연결 문제)
            var errorMessage = request.error;

            Debug.LogError($"Request failed. StatusCode: {statusCode}, Error: {errorMessage}");

            throw new HttpRequestException(
                $"HTTP Request failed. Status Code: {statusCode}, Error Message: {errorMessage}"
            );
        }

        return request;
    }

    private ApiResult<T> BuildApiResult<T>(T data, string message = "", bool isSuccess = true)
    {
        return new ApiResult<T>
        {
            IsSuccess = isSuccess,
            Message = message,
            Data = data
        };
    }

    private string BuildEndpoint(string endpoint) => $"{_baseUrl}/{endpoint}";

    public async UniTask<ApiResult<T>> GetAsync<T>(string endpoint)
    {
        endpoint = BuildEndpoint(endpoint);
        using (UnityWebRequest request = UnityWebRequest.Get(endpoint))
        {
            try
            {
                var response = await SendRequestAsync(request);
                var data = JsonConvert.DeserializeObject<T>(response.downloadHandler.text);
                return BuildApiResult(data);
            }
            catch (Exception ex)
            {
                return BuildApiResult<T>(default, ex.Message, false);
            }
        }
    }

    public async UniTask<ApiResult<T>> PutAsync<T>(string endpoint, object data)
    {
        endpoint = BuildEndpoint(endpoint);
        var jsonData = data == null ? null : JsonConvert.SerializeObject(data);
        using (UnityWebRequest request = UnityWebRequest.Put(endpoint, jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            try
            {
                var response = await SendRequestAsync(request);
                var result = JsonConvert.DeserializeObject<T>(response.downloadHandler.text);
                return BuildApiResult(result);
            }
            catch (Exception ex)
            {
                return BuildApiResult<T>(default, ex.Message, false);
            }
        }
    }

    public async UniTask<ApiResult<T>> PostAsync<T>(string endpoint, object data)
    {
        endpoint = BuildEndpoint(endpoint);
        var jsonData = data == null ? null : JsonConvert.SerializeObject(data);
        Debug.Log($"PostAsync: {endpoint}, {jsonData}");

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] jsonToSend = jsonData is not null ? Encoding.UTF8.GetBytes(jsonData) : null;
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            try
            {
                var response = await SendRequestAsync(request);
                var result = JsonConvert.DeserializeObject<T>(response.downloadHandler.text);
                return BuildApiResult(result);
            }
            catch (Exception ex)
            {
                return BuildApiResult<T>(default, ex.Message, false);
            }
        }
    }

    public async UniTask<ApiResult<bool>> DeleteAsync(string endpoint)
    {
        endpoint = BuildEndpoint(endpoint);
        using (UnityWebRequest request = UnityWebRequest.Delete(endpoint))
        {
            try
            {
                await SendRequestAsync(request);
                return BuildApiResult(true);
            }
            catch (Exception ex)
            {
                return BuildApiResult(false, ex.Message, false);
            }
        }
    }
}


