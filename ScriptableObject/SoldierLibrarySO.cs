using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoldierLibrarySO",menuName = "Soldier/SoldierLibrarySO")]
public class SoldierLibrarySO: ScriptableObject
{
    public List<SoldierLibraryEntry> soldierLibraryList;
}

[System.Serializable]
public struct SoldierLibraryEntry
{
    public SoldierDataSO soldierData;
    public int amount;
}
