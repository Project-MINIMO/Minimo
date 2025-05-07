using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using MinimoShared;
using UniRx;
using UnityEngine;

public class AccountInfoManager : ManagerBase
{
    public ReactiveProperty<string> NickName { get; } = new ReactiveProperty<string>();
    public ReactiveProperty<int> Level { get; } = new ReactiveProperty<int>();
    public ReactiveProperty<int> Star { get; } = new ReactiveProperty<int>();
    public ReactiveProperty<int> BlueStar { get; } = new ReactiveProperty<int>();
    
    private GameClient _gameClient;
    
    private void Start()
    {
        _gameClient = App.Services.GetRequiredService<GameClient>();
    }

    public void Setup()
    {
        NickName.Value = _gameClient.AccountInfo.Nickname;
        Level.Value = _gameClient.AccountInfo.Level;
        Star.Value = _gameClient.AccountInfo.Currency.Star;
        BlueStar.Value = _gameClient.AccountInfo.Currency.BlueStar;
    }
    
    public void UpdateAccountInfo(AccountDTO account)
    {
        _gameClient.AccountInfo = account;
    }
    
    public void UpdateNickname(string nickname)
    {
        _gameClient.AccountInfo.Nickname = nickname;
        NickName.Value = nickname;
    }
    
    public void UpdateCurrency(CurrencyDTO currency)
    {
        _gameClient.AccountInfo.Currency = currency;
        BlueStar.Value = currency.BlueStar;
        Star.Value = currency.Star;
    }
    
    public void UpdateLevel(int level)
    {
        _gameClient.AccountInfo.Level = level;
        Level.Value = level;
    }

    public void UpdateBuilding(BuildingDTO buildingDto)
    {
        var building = _gameClient.AccountInfo.Buildings.Find(b => b.Id == buildingDto.Id);
        if(building != null)
        {
            building.CopyFrom(buildingDto);
        }
        else
        {
            _gameClient.AccountInfo.Buildings.Add(buildingDto);
        }
    }
    
    public void DeleteBuilding(int buildingId)
    {
        var building = _gameClient.AccountInfo.Buildings.Find(b => b.Id == buildingId);
        if(building != null)
        {
            _gameClient.AccountInfo.Buildings.Remove(building);
        }
    }
    
    public void UpdateBuildingInfo(BuildingInfoDTO buildingInfoDto)
    {
        var buildingInfo = _gameClient.AccountInfo.BuildingInfos.Find(b => b.BuildingType == buildingInfoDto.BuildingType);
        if(buildingInfo != null)
        {
            buildingInfo.CopyFrom(buildingInfoDto);
        }
        else
        {
            _gameClient.AccountInfo.BuildingInfos.Add(buildingInfoDto);
        }
    }
    
    public void UpdateItem(ItemDTO itemDto)
    {
        var item = _gameClient.AccountInfo.Items.Find(i => i.ItemType == itemDto.ItemType);
        if(item != null)
        {
            item.CopyFrom(itemDto);
        }
        else
        {
            _gameClient.AccountInfo.Items.Add(itemDto);
        }
    }
    
    public void UpdateItems(List<ItemDTO> items)
    {
        foreach(var itemDto in items)
        {
            UpdateItem(itemDto);
        }
    }
    
    public void AddItemCount(string itemType, int count)
    {
        var item = _gameClient.AccountInfo.Items.Find(i => i.ItemType == itemType);
        if (item != null) 
        {
            item.Count += count;
        }
        else
        {
            var newItem = new ItemDTO
            {
                ItemType = itemType,
                Count = count
            };
            _gameClient.AccountInfo.Items.Add(newItem);
        }

        App.GetManager<CheatManager>().UpdateItem(item);
    }
    
    public void AddItemCount(int itemType, int count)
    {
        
    }
    
    public ItemDTO GetItem(string itemType)
    {
        var item = _gameClient.AccountInfo.Items.Find(i => i.ItemType == itemType);
        if (item != null) 
        {
            return item;
        }
        else
        {
            var newItem = new ItemDTO
            {
                ItemType = itemType,
                Count = 0
            };
            _gameClient.AccountInfo.Items.Add(newItem);

            return newItem;
        }
    }
    
    public ItemDTO GetItem(int itemType)
    {
        return null;
    }
    
    public void UpdateAssetInfo(AssetUpdateDTO assetUpdate)
    {
        if(assetUpdate.CurrencyUpdate != null)
        {
            UpdateCurrency(assetUpdate.CurrencyUpdate.CurrentCurrency);
        }
        if(assetUpdate.BuildingsUpdate != null)
        {
            foreach(var buildingUpdateInfo in assetUpdate.BuildingsUpdate)
            {
                UpdateBuildingInfo(buildingUpdateInfo.CurrentBuildingInfo);
            }
        }
        if(assetUpdate.ItemsUpdate != null)
        {
            foreach(var itemUpdate in assetUpdate.ItemsUpdate)
            {
                UpdateItem(itemUpdate.CurrentItem);
            }
        }
    }
    
    public void UpdateLastMeteorCreatedAt(DateTime lastMeteorCreatedAt)
    {
        _gameClient.AccountInfo.LastMeteorCreatedAt = lastMeteorCreatedAt;
    }
    
    public void AddMeteor(MeteorDTO meteorDto)
    {
        _gameClient.AccountInfo.Meteors.Add(meteorDto);
    }
    
    public void AddMeteors(List<MeteorDTO> meteorDtos)
    {
        _gameClient.AccountInfo.Meteors.AddRange(meteorDtos);
    }

    public void RemoveMeteor(int meteorId)
    {
        var meteor = _gameClient.AccountInfo.Meteors.Find(m => m.Id == meteorId);
        _gameClient.AccountInfo.Meteors.Remove(meteor);
    }
    
    public void AddQuest(QuestDTO questDto)
    {
        _gameClient.AccountInfo.Quests.Add(questDto);
    }
    
    public void UpdateLastStarTreeCreatedAt(DateTime lastStarTreeCreatedAt)
    {
        _gameClient.AccountInfo.LastStarTreeCreatedAt = lastStarTreeCreatedAt;
    }
    
    public void UpdateLastWishedAt(DateTime lastWishedAt)
    {
        _gameClient.AccountInfo.LastWishedAt = lastWishedAt;
    }
}
