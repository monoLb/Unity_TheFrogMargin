using UnityEngine;

[CreateAssetMenu(fileName = "PoisonBehaviour", menuName = "Buff/Behaviour/PoisonBehaviour")]
public class PoisonBehaviourSO : DamageOverTimeBehaviourSO
{
    public float hpRate=0.1f;

    [Header("Rogue")] 
    public bool ableStack;

    public bool isMaxHp;

    protected override int CalculateTickDamage(BuffInstance buff)
    {
        if (buff == null || buff.owner == null)
            return 0;

        // 1. 计算基础 HP 值（规则唯一入口）
        float baseHp = isMaxHp ? buff.owner.maxHp : buff.owner.hp;

        // 2. 基础伤害（不包含任何肉鸽 / 叠层）
        float baseDamage = baseHp * hpRate;

        // 3. 累乘修正器（肉鸽核心）TODO 后期
        float multiplier = 1f;

        // 3.1 叠层修正
        if (ableStack)
        {
            multiplier += 0.25f * buff.stackCount;
        }

        // 4. 最终结算
        float finalDamage = baseDamage * multiplier;

        return Mathf.Max(1, Mathf.RoundToInt(finalDamage));
    }
}