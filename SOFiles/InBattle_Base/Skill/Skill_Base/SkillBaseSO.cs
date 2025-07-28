using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public enum CountType
{
    FixedNumber, 
    All
}
public abstract class SkillBaseSO : ScriptableObject
{
    [Header("Base Info")]
    public string skillName;
    [TextArea]
    public string description;
    public SkillType skillType;
        
    [Header("Value Calculator")]
    public float attBonus;
    public float magBonus;
    public float rageBonus=1;

    [Header("Partical Effect")] 
    public GameObject hitEffectPrefab;
    
    [Header("Buff/Extra")]
    public List<BuffBaseSO> buffs;
    public List<ExtraBaseSO> extras;
    
    [Header("Set")] 
    public int targetCount;
    

    /// <summary>
    /// 技能执行逻辑，caster：施法者
    /// </summary>
    public abstract IEnumerator Activate(SoldierInBattle caster, List<SoldierInBattle> targets);

    public virtual AttackTargetType GetAttackTargetType() => AttackTargetType.Default; // 默认或抛异常
    public virtual SupportTargetType GetSupportTargetType() => SupportTargetType.Random;// 默认或抛异常
    public virtual FactionType GetSupportFactionType() => FactionType.Friend;
    protected int ValueCalculator(int att, int mag, int rage)
    {
        float baseDamage = att * attBonus + mag * magBonus;
        float rageFactor = (rage / 100f) * rageBonus;

        float totalDamage = baseDamage * rageFactor;
        return Mathf.RoundToInt(totalDamage);
    }

}
