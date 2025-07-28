using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SoldierLibrarySO",menuName = "Soldier/SoldierLibrarySO")]
public class StoreSoldierLibrarySO: ScriptableObject
{
    public List<StoreSoldierLibraryEntry> SoldierLibraryList;
}


[System.Serializable]
public struct StoreSoldierLibraryEntry
{
    public SoldierDataSO soldierData;
    public int amount;
}
