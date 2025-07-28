using System.Collections;
using UnityEngine;

public class BuffBehaviourSO : ScriptableObject
{
    /// <summary>
    /// 优先级 越小越优先
    /// </summary>
    /// 
    public int priority;
    
    // === 回合 ===
    public virtual IEnumerator OnTurnStart(BuffInstance buff){yield break;}
    public virtual IEnumerator OnTurnEnd(BuffInstance buff){yield break;}
    
    // === 行动 ===
    
    // === 攻击行为的完整延展 ===
    /// <summary>
    /// 是否允许行动
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="isBlocked"></param>
    public virtual void OnCheckActionBlock(BuffInstance buff,ref bool isBlocked){}

    /// <summary>
    /// 是否背叛
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="isBetrayed"></param>
    public virtual void OnCheckActionBetray(BuffInstance buff,ref bool isBetrayed){}
    
    /// <summary>
    /// 是否行动受限
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="isSilence"></param>
    /// <param name="isDisarm"></param>
    public virtual void OnCheckActionRestriction(BuffInstance buff,ref bool isSilence,ref bool isDisarm){}
    
    // === 一个伤害的前后Buff ===
    
    /// <summary>
    /// 收到伤害前执行
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="ctx"></param>
    public virtual IEnumerator OnBeforeDamage(BuffInstance buff){yield break;}
    
    /// <summary>
    /// 受到伤害后执行
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public virtual IEnumerator OnAfterDamaged(BuffInstance buff){yield break;}
    
    
    // === 伤害修正 ==
    /// <summary>
    /// 修正造成的伤害
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual void ModifyOutgoingDamage(BuffInstance buff){}
    
    /// <summary>
    /// 修正收到的伤害
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual void ModifyIncomingDamage(BuffInstance buff,DamageContext ctx){}
    
    // === 状态 ===
    public virtual void OnRemoved(BuffInstance buff){}
}
