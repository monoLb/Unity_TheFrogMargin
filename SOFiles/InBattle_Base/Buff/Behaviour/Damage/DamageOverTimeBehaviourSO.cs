using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class DamageOverTimeBehaviourSO : DamageBuffBaseSO
{
    
    protected abstract int CalculateTickDamage(BuffInstance buff);

    public override IEnumerator OnTurnStart(BuffInstance buff)
    {
        if (buff == null || buff.owner == null)
            yield break;

        int dmg = 1 + CalculateBaseDamage(buff) + CalculateTickDamage(buff);

        var ctx = new DamageContext(dmg, DamageType.BuffDamage);
        yield return buff.owner.TakeDamage(ctx);

        if (buff.stackCount >= buff.source.maxStack)
        {
            buff.stackCount = 0;
            //爆炸行为 也可以拆解为第二个行为
        }
    }

    
}