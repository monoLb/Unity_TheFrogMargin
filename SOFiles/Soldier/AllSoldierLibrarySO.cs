using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "AllSoldierLibrarySO", menuName = "Soldier/AllSoldierLibrarySO")]
public class AllSoldierLibrarySO : ScriptableObject
{
    public List<PartSoldierLibrary> CommonLibrary;
    public List<PartSoldierLibrary> StageLibrarary;
    public List<PartSoldierLibrary> ChapterLibrary;
}
[System.Serializable]
public class PartSoldierLibrary
{
    public string StageName;
    public List<SoldierDataSO> LibraryList;
}


