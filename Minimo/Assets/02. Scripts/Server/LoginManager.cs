using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MinimoShared;
using Newtonsoft.Json.Linq;

using UnityEngine;

[DefaultExecutionOrder(-30)]
public class LoginManager : ManagerBase
{
    private GameClient _gameClient;
    
    [SerializeField] private string _userName = "";
    [SerializeField] private string _password = "";
    [SerializeField] private string _nickname = "";
    
    private readonly string _loginEndpoint = "api/login";
    private readonly string _accountEndPoint = "api/accounts";
    
    public string JwtToken { get; private set; }
    private bool IsLoggedIn { get; set; } = false;
    
    private void Start()
    {
        Debug.Log("LoginManager initialized.");
        _gameClient = App.Services.GetRequiredService<GameClient>();
        JwtToken = PlayerPrefs.GetString("JwtToken");
    }
    
    public void Logout()
    {
        JwtToken = "";
        PlayerPrefs.DeleteKey("JwtToken");
        PlayerPrefs.Save();
        IsLoggedIn = false;
    }
    
    public async UniTask<ApiResult<JObject>> LoginAsync(string username, string password)
    {
        if(IsLoggedIn)
        {
            return new ApiResult<JObject> { IsSuccess = false, Message = "Already Logged In" };
        }
        
        LoginDTO loginDto = new LoginDTO { Username = username, Password = password };
        var result = await _gameClient.PostAsync<JObject>(_loginEndpoint, loginDto);
        if(result.IsSuccess && result.Data is {} response)
        {
            var token = response["token"].Value<string>();
            var time = response["time"].Value<DateTime>();
            var accountDTO = response["account"].ToObject<AccountDTO>();
            
            App.GetManager<TimeManager>().Init(time);
            _gameClient.AccountInfo = accountDTO;
            _nickname = accountDTO.Nickname;
            JwtToken = token;
            PlayerPrefs.SetString("JwtToken", JwtToken);
            PlayerPrefs.Save();
            IsLoggedIn = true;

            return result;

        }
        else
        {
            Debug.LogError($"Login Failed: {result.Message}");
            return result;
        }   
    }

    public async UniTask<ApiResult<JObject>> LoginWithTokenAsync()
    {
        if(IsLoggedIn)
        {
            return new ApiResult<JObject> { IsSuccess = false, Message = "Already Logged In" };
        }
        
        var endpoint = _loginEndpoint + "/token";
        var result = await _gameClient.PostAsync<JObject>(endpoint, null);
        if(result.IsSuccess && result.Data is {} response)
        {
            var time = response["time"].Value<DateTime>();
            var accountDTO = response["account"].ToObject<AccountDTO>();
            
            App.GetManager<TimeManager>().Init(time);
            _gameClient.AccountInfo = accountDTO;
            _nickname = accountDTO.Nickname;
            IsLoggedIn = true;
            
            Debug.Log($"Login With Token Success: Token: {JwtToken} Time: {time} NickName: {_nickname}");
            return result;
        }
        else
        {
            Debug.LogWarning($"Login Failed: {result.Message}");
            // delete token
            PlayerPrefs.DeleteKey("JwtToken");
            PlayerPrefs.Save();
            return result;
        }
    }
    
    public async UniTask<ApiResult<AccountDTO>> CreateAccountAsync(string username, string password, string nickname)
    {
        var createAccountDto = new CreateAccountDTO { Username = username, Password = password, Nickname = nickname };
        return await _gameClient.PostAsync<AccountDTO>(_accountEndPoint, createAccountDto);
    }
}
