using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.Serialization;

[System.Serializable]
public class SaveData
{
    public string playerName;
    public int playerMoney;
    public int chapter;
    public int floor;
    public int maxBattleSoldierCount;
    
    public MapData mapData = new();
    public bool hadKey;

    public List<SoldierInstance> teamList = new();
    public List<SoldierInstance> battleList = new();
    
    public InventoryItemData[] storageArray;
    public InventoryItemData[] bagArray;
    public InventoryItemData[] mapOpenerArray;
    
    public readonly int StoragePageSize = 24;
    public readonly int MapOpenerSize = 4;
    
    public SaveData(int BagSize)
    {
        storageArray = new InventoryItemData[StoragePageSize];
        bagArray = new InventoryItemData[BagSize];
        mapOpenerArray = new InventoryItemData[MapOpenerSize];
        
        maxBattleSoldierCount = 3;

        
        for (int i = 0; i < StoragePageSize; i++)
        {
            storageArray[i] = new InventoryItemData();
        }
        for (int i = 0; i < BagSize; i++)
        {
            bagArray[i] = new InventoryItemData();
        }
        for (int i = 0; i < MapOpenerSize; i++)
        {
            mapOpenerArray[i] = new InventoryItemData();
        }
        
    }
}