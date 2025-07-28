using System.Collections;
using UnityEngine;

public abstract class DamageAfterBehaviourSO : DamageBuffBaseSO
{
    protected abstract int CalculateTriggerDamage(BuffInstance buff);

    public override IEnumerator OnAfterDamaged(BuffInstance buff)
    {
        if (buff == null || buff.owner == null)
            yield break;

        int dmg = CalculateBaseDamage(buff) + CalculateTriggerDamage(buff);

        var ctx = new DamageContext(dmg, DamageType.BuffDamage);
        yield return buff.owner.TakeDamage(ctx);
        
    } 
}
