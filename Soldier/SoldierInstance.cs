using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SoldierInstance
{
    public string uniqueID;
    public SoldierDataSO soldierData;

    public int health;
    public int attack;
    public int magic;
    public int speed;
    
    public QualityType quality;
    public List<ElementType> elementList;

    public InventoryItemData[] equippedRune;
    
    [Header("State")] 
    public bool isBattle;
    public int formationIndex = -1;
    

    public SoldierInstance(){}

    public SoldierInstance(SoldierDataSO data, int atk, int magic, int hp, int speed,
        List<ElementType> elementList)

    {
        this.uniqueID = System.Guid.NewGuid().ToString();
        this.soldierData = data;
        this.attack = atk;
        this.magic = magic;
        this.health = hp;
        this.speed = speed;

        this.equippedRune = new InventoryItemData[1];
        equippedRune[0] = new InventoryItemData(); // 防止null

        this.elementList = elementList;
    }
}
