using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using DG.Tweening;
using Microsoft.Extensions.DependencyInjection;

public enum SceneName
{
    Developer,
    Title,
    Prolog,
    Game
}

// TODO (하람's 질문) : 현재 구현된 바로는 ManaverBase와 DataBase는 동일한 클래스인데 이름만 다른 것으로 보입니다. 데이터 클래스와 매니저 클래스를 이름이 아니라 클래스를 아예 구분하는 이유가 궁금합니다...!
[DefaultExecutionOrder(-100)]
public class App : Singleton<App>
{
    private static readonly Dictionary<Type, MonoBehaviour> _managers = new();
    private static readonly Dictionary<Type, MonoBehaviour> _datas = new();
    public static IServiceProvider Services { get; private set; }
    
    private static BottomNotification _notification;
    private static BlackScreen _blackScreen;
    public static LoadingSpinner Loading;

    protected override void Awake()
    {
        base.Awake();

        _notification = GetComponentInChildren<BottomNotification>();
        _blackScreen = GetComponentInChildren<BlackScreen>();
        Loading = GetComponentInChildren<LoadingSpinner>();
        
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 120;

        DOTween.safeModeLogBehaviour = DG.Tweening.Core.Enums.SafeModeLogBehaviour.Error;
    }

    private static void Register(MonoBehaviour obj, Dictionary<Type, MonoBehaviour> dictionary) 
        => dictionary[obj.GetType()] = obj;

    public static void RegisterManager(MonoBehaviour manager)
    {
        Register(manager, _managers);
    }

    public static void RegisterData(MonoBehaviour data)
    {
        Register(data, _datas);
    }

    public static T GetManager<T>() where T : MonoBehaviour
    {
        var type = typeof(T);

        if (_managers.TryGetValue(type, out MonoBehaviour manager))
        {
            return manager as T;
        }

        Debug.LogWarning($"Manager of type {type.Name} not found.");
        return null;
    }

    public static T GetData<T>() where T : MonoBehaviour
    {
        var type = typeof(T);

        if (_datas.TryGetValue(type, out MonoBehaviour data))
        {
            return data as T;
        }

        Debug.LogWarning($"Data of type {type.Name} not found.");
        return null;
    }

    public static void LoadScene(SceneName sceneName)
    {
        GetManager<SoundManager>().FadeOutBGM(2);
        _blackScreen.FadeInOut(2f, () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene((int)sceneName);

            if (sceneName == SceneName.Title)
            {
                GetManager<SoundManager>().PlayBGM("Title");
            }
            else if (sceneName == SceneName.Game)
            {
                GetManager<SoundManager>().PlayBGM("InGame");
            }
        });

    }
    
    public static void LogBox(string titleColor, string title, Dictionary<string, string> contents)
    {
        var logString = string.Empty;
        logString += $"<color={titleColor}>{title}</color>";
        foreach (var pair in contents)
        {
            logString += $"\n<color={titleColor}>▶</color> <b>{pair.Key}</b> : {pair.Value}";
        }
        Debug.Log(logString);
    }

    public static void Notification(NotifyType type)
    {
        _notification.ShowNotification(type);
    }

    public static void FadeInOut(float duration, Action callback)
    {
        _blackScreen.FadeInOut(duration, callback);
    }
}