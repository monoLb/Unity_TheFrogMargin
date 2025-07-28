using UnityEngine;

public enum DamageType
{
    CommonAtkDamage,     // 普通攻击
    SkillDamage,   // 技能
    BuffDamage,    // 持续伤害（毒、火、流血）
    ExtraDamage    // 额外触发伤害（连锁、反击）
}

public class DamageContext
{
    public int baseValue; // 收到的伤害原始数值
    
    public DamageType type;
    
    // 数值修正
    public bool isBlocked;
    public int finalValue;

    public DamageContext(DamageType type)
    {
        this.type = type;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="baseValue">基础数值</param>
    /// <param name="type">伤害类型</param>
    public DamageContext(int baseValue, DamageType type)
    {
        this.baseValue = baseValue;
        this.type = type;
    }
}
