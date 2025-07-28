using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class PlayerSaveData : MonoBehaviour
{
    public static PlayerSaveData Instance;
    private const string PLAYER_DATA_FILE_NAME = "PlayerData.data";

    public SaveData globalData;

    /// <summary>
    /// 背包部分
    /// </summary>
    public int inventorySize = 20;
    
    public IntEventSO playerMoneyEvent;

    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持物体切换场景时不销毁
            
        }
        else
        {
            Destroy(gameObject); // 只保留一个
        }
    }
    

    public void MoneyChanged(int money,bool isAdd)
    {
        if (isAdd)
            globalData.playerMoney += money;
        else
            globalData.playerMoney -= money;
        
        playerMoneyEvent.RaiseEvent(globalData.playerMoney,this);
    }
    

    
    public void Save()
    {
        SaveSystem.SaveByJson(PLAYER_DATA_FILE_NAME,globalData);
    }

    public void Load()
    {
        SaveSystem.LoadByJson<SaveData>(PLAYER_DATA_FILE_NAME);
    }
    
}
