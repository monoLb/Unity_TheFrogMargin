using UnityEngine;

public class DamageBuffBaseSO : BuffBehaviourSO
{
    [Header("Caster Ratio")]
    public float casterHpRatio;
    public float casterAttRatio;
    public float casterMagRatio;
    public float casterSpeRatio;
    [Header("Target Ratio")]
    public float targetHpRatio;
    public float targetAttRatio;
    public float targetMagRatio;
    public float targetSpeRatio;
    
    [Header("Rogue setting")] 
    public bool stackAble;
    
    protected int CalculateBaseDamage(BuffInstance buff)
    {
        if (buff == null ||
            buff.casterAttributes == null ||
            buff.owner == null)
            return 0;
        
        float damage = 0f;

        // caster
        damage += buff.casterAttributes.hp  * casterHpRatio;
        damage += buff.casterAttributes.att * casterAttRatio;
        damage += buff.casterAttributes.mag * casterMagRatio;
        damage += buff.casterAttributes.spe * casterSpeRatio;

        // owner
        damage += buff.owner.hp  * targetHpRatio;
        damage += buff.owner.att * targetAttRatio;
        damage += buff.owner.mag * targetMagRatio;
        damage += buff.owner.spe * targetSpeRatio;

        // 统一结算规则（DOT 不允许 0 伤害）
        return Mathf.Max(1, Mathf.FloorToInt(damage));
    }
}
