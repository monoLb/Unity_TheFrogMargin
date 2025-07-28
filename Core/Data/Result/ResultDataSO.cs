using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResultDataSO", menuName = "Core/ResultDataSO")]
public class ResultDataSO : ScriptableObject
{
   public List<ResultItem> currencies= new List<ResultItem>();
}

[System.Serializable]
public class ResultItem
{
    public string Result_Name;
    public int Result_Value;
    public Sprite Result_Sprite;
}
