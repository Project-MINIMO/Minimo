using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private TitlePanel _titlePanel;
    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private TMP_InputField _pwInputField;
    [SerializeField] private Button _loginBtn;
    [SerializeField] private Button _registerBtn;
    [SerializeField] private Button _guetsLogin;
    [SerializeField] private TextMeshProUGUI _resultText;
    
    //private bool isLogin;
    
    private async void Start()
    {
        _loginBtn.onClick.AddListener(() =>
        {
            _resultText.text = string.Empty;
            OnLogin();
        });
        _registerBtn.onClick.AddListener(OnRegister);
        
        /*
        _loginManager = App.GetManager<LoginManager>();
        if (!string.IsNullOrEmpty(_loginManager.JwtToken))
        {
            var result = await _loginManager.LoginWithTokenAsync();
            if (result.IsSuccess)
            {
                await _titlePanel.ShowTitle();
            }
            else
            {
                _loginPanel.SetActive(true);
            }
        }
        else
        {
            _loginPanel.SetActive(true);
        }
        */
    }
    
    private async void OnLogin(bool isNew = false)
    {
        var id = ThrowHelper.IfNullOrWhitespace(_idInputField.text);
        var pw = ThrowHelper.IfNullOrWhitespace(_pwInputField.text);

        /*
        // 로그인 요청
        var result = await _loginManager.LoginAsync(id, pw);
        if (result.IsSuccess)
        {
            await _titlePanel.ShowTitle(isNew);
            _loginPanel.SetActive(false);
        }
        else
        {
            _resultText.text = result.Message;
        }
        */
    }
    
    private async void OnRegister()
    {
        var id = ThrowHelper.IfNullOrWhitespace(_idInputField.text);
        var pw = ThrowHelper.IfNullOrWhitespace(_pwInputField.text);
        var randomNickname = "User" + UnityEngine.Random.Range(0, 1000);
        
        /*
        // 회원가입 요청
        var result = await _loginManager.CreateAccountAsync(id, pw, randomNickname);
        if (result.IsSuccess) 
        {
            OnLogin(true);
        }
        else
        {
            _resultText.text = result.Message;
        }
        
        Debug.Log(result.Data);
        */
    }
}

public static class ThrowHelper
{
    public static string IfNullOrWhitespace(string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentException("Argument is whitespace");
        }

        return argument;
    }
}