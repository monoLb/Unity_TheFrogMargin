using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SoldierDataSO",menuName = "Soldier/SoldierDataSO")]
public class SoldierDataSO : ScriptableObject
{
   
   public string soldier_Name;
   public QualityType soldier_Quantity;
   
   public FormationLineType formationLineType;
   
   public Sprite soldier_Sprite;
   [TextArea]
   public string soldier_Talk;
   public int soldier_Price;

   public float soldier_Attack;
   public float soldier_Health;
   public float soldier_Speed;
   public float soldier_Magic;
   
   public string soldier_SkillName;
   [TextArea]
   public string soldier_SkillDescription;
   //skill类
   public SkillBaseSO soldier_Skill;
   
}

