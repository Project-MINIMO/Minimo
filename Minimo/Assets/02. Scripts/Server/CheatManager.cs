using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MinimoShared;
using UnityEngine;

public class CheatManager : ManagerBase
{
    private GameClient _gameClient;
    private AccountInfoManager _accountInfoManager;

    private const string CheatEndpoint = "api/cheat";

    private void Start()
    {
        _gameClient = App.Services.GetRequiredService<GameClient>();
        _accountInfoManager = App.GetManager<AccountInfoManager>();
    }
    
    public async UniTask UpdateCurrency(CurrencyDTO targetCurrency)
    {
        var endPoint = $"{CheatEndpoint}/currency";
        var result = await _gameClient.PostAsync<CurrencyDTO>(endPoint, targetCurrency);
        if(result.IsSuccess && result.Data is {} currency)
        {
            Debug.Log($"Currency updated: {currency}");
            _accountInfoManager.UpdateCurrency(currency);
            return;
        }
        else
        {
            Debug.LogError($"Failed to update currency: {result.Message}");
            return;
        }
    }
    
    public async UniTask UpdateBuildingInfo(BuildingInfoDTO targetBuildingInfo)
    {
        var endPoint = $"{CheatEndpoint}/buildinginfo";
        var result = await _gameClient.PostAsync<BuildingInfoDTO>(endPoint, targetBuildingInfo);
        if(result.IsSuccess && result.Data is {} buildingInfo)
        {
            Debug.Log($"Building info updated: {buildingInfo}");
            _accountInfoManager.UpdateBuildingInfo(buildingInfo);
            return;
        }
        else
        {
            Debug.LogError($"Failed to update building info: {result.Message}");
            return;
        }
    }

    public async UniTask UpdateItem(ItemDTO targetItem)
    {
        var endPoint = $"{CheatEndpoint}/item";
        var result = await _gameClient.PostAsync<ItemDTO>(endPoint, targetItem);
        if(result.IsSuccess && result.Data is {} item)
        {
            Debug.Log($"Item updated: {item}");
            _accountInfoManager.UpdateItem(item);
            return;
        }
        else
        {
            Debug.LogError($"Failed to update item: {result.Message}");
            return;
        }
    }
    
    public async void SetServerTimeForce(string targetTime)
    {
        var timeManager = App.GetManager<TimeManager>();
        if (timeManager.IsProcessing == false)
        {
            Debug.LogWarning("TimeManager is not processing");
            return;
        }
        
        if (DateTime.TryParse(targetTime, out var time))
        {
            var result = await _gameClient.PutAsync<DateTime>("api/time", time);
            if (result.IsSuccess && result.Data is { } newServerTime)
            {
                timeManager.SyncTime(newServerTime);
            }
            else
            {
                Debug.LogWarning("Failed To Set Cheat Time: {result.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Invalid time format");
        }
    }

    public async void TryCreateMeteor()
    {
        var result = await App.GetManager<MeteorManager>().CreateMeteor();
    }
    
    public async void TryGetMeteorResult(int meteorId)
    {
        var result = await App.GetManager<MeteorManager>().GetMeteorResult(meteorId);
    }
}
