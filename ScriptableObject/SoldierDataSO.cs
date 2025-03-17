using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SoldierDataSO",menuName = "Soldier/SoldierDataSO")]
public class SoldierDataSO : ScriptableObject
{
   public string soldier_Name;
   public Sprite soldier_Sprite;
   [TextArea]
   public string soldier_Description;
   public SoldierType soldier_Type;
   public int soldier_Slot;

   public float soldier_Attack;
   public float soldier_Health;
   public float soldier_Speed;
   public string soldier_Skill;
}

public enum SoldierType
{
   Warrior,
   Wizard,
   Healer
}

public enum Faction
{
   friend,
   enemy
}