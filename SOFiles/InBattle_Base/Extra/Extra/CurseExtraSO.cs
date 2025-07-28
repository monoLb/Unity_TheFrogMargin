using System;
using System.Collections;
using UnityEngine;

public enum CurseExtraType
{
    SelfExplode,
    RageSteal,
    BreakShield, // 破盾
}
[CreateAssetMenu(fileName = "CurseExtra", menuName = "Effect/Exta/CurseExtra")]
public class CurseExtraSO : ExtraBaseSO
{
    public CurseExtraType curseType;
    public override IEnumerator ExtraActivate(SoldierInBattle caster, SoldierInBattle target,float value)
    {
        
        var ctx = new DamageContext(DamageType.ExtraDamage);
        
        switch (curseType)
        {
            case CurseExtraType.SelfExplode:
                ctx.baseValue = caster.hp;
                yield return caster.TakeDamage(ctx);
                break;
            case CurseExtraType.RageSteal:
                break;
            case CurseExtraType.BreakShield:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        yield return null;
    }
}
