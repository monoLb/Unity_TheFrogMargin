using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public enum BlessingExtraType
{
    LifeSteal, // 吸血
    AddRage,
    StrengthenShield, // 增盾
    DivineShield, // 圣盾
    Batter, 
    Evolve
}

[CreateAssetMenu(fileName = "BlessingExtra", menuName = "Effect/Exta/BlessingExtra")]
public class BlessingExtraSO : ExtraBaseSO
{
    public BlessingExtraType extraType;
    public override IEnumerator ExtraActivate(SoldierInBattle caster, SoldierInBattle target,float value)
    {
        Debug.Log(extraType);
        yield return ApplyEffect(caster, target);
        switch (extraType)
        {
            case BlessingExtraType.LifeSteal:
                caster.ReceiveCause(CauseType.Heal,Mathf.RoundToInt(value*extraAddValue));
                break;
            case BlessingExtraType.AddRage:
                caster.rage += addRage;
                break;
            case BlessingExtraType.StrengthenShield:
                break;
            case BlessingExtraType.DivineShield:
                break;
            case BlessingExtraType.Batter: //反弹
                break;
            case BlessingExtraType.Evolve: //进化
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    

    private IEnumerator ApplyEffect(SoldierInBattle caster, SoldierInBattle target)
    {
    
        float effectDuration = 1.0f; // 你可以根据特效动画时长调整
        if (targetEffectPrefab != null) 
        {
            GameObject targetEffect = Instantiate(targetEffectPrefab, target.transform);

            targetEffect.transform.localPosition = new Vector3(0, 4.76f, -2.84f);
            Debug.Log(targetEffect+"触发Extra");
            //targetEffect.transform.DOScale(Vector3.zero, effectDuration).SetEase(Ease.InBack).OnComplete(() => GameObject.Destroy(targetEffect));
        }

        if (casterEffectPrefab != null) 
        {
            GameObject casterEffect = Instantiate(base.casterEffectPrefab, caster.transform);
        }
        
        yield return null;
        
    }
}
