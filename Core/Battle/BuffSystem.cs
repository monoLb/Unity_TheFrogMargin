using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BuffSystem
{
    private SoldierInBattle owner;
    private List<BuffInstance> activeBuffs;
    
    public BuffSystem(SoldierInBattle owner, List<BuffInstance> buffList)
    {
        this.owner = owner;
        this.activeBuffs = buffList;
    }

    /// <summary>
    /// 回合开始（负面Buff）
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartOfTurnBuffs()
    {
        if(owner.isDead || activeBuffs.Count == 0)
            yield break;

        var sortedBuffs = SortBuffList(BuffType.TurnStartBuff);

        foreach (var buff in sortedBuffs)
        {
            if (buff.source.behaviours == null) 
                continue;

            foreach (var behaviour in buff.source.behaviours.OrderBy(b => b.priority))
            {
                yield return behaviour.OnTurnStart(buff);
            }
            
            if (buff.IsExpired)
            {
                activeBuffs.Remove(buff);
            }
            
            owner.RefreshBuffGroup();
            yield return new WaitForSeconds(0.1f);
        }
    }

    
    /// <summary>
    /// 回合结束（正面Buff）
    /// </summary>
    /// <returns></returns>
    public IEnumerator EndOfTurnBuff()
    {
        if(owner.isDead || activeBuffs.Count == 0)
            yield break;
        
        var sortedBuffs = SortBuffList(BuffType.TurnEndBuff);

        foreach (var buff in sortedBuffs)
        {
            if (buff.source.behaviours == null) 
                continue;
            
            foreach (var behaviour in buff.source.behaviours.OrderBy(b => b.priority))
            {
                yield return behaviour.OnTurnEnd(buff);
            }
            
            if (buff.IsExpired)
            {
                activeBuffs.Remove(buff);
            }

            owner.RefreshBuffGroup();
            yield return new WaitForSeconds(0.1f);

        }
    }
    
    /// <summary>
    /// 受到伤害前执行（免疫 / 护盾）
    /// </summary>
    public IEnumerator OnBeforeDamageBuff(DamageContext ctx)
    {
        if(activeBuffs.Count == 0)
            yield break;
        
        var sortedBuffs = SortBuffList(BuffType.BeforeDamageBuff);
        
        foreach (var buff in sortedBuffs)
        {
            if (buff.source.behaviours == null) 
                continue;

            foreach (var beh in buff.source.behaviours.OrderBy(b => b.priority))
            {
                
                yield return beh.OnBeforeDamage(buff);
               
            }
            
        }
    }
    
    /// <summary>
    /// 受到伤害后执行（特殊Buff）
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public IEnumerator OnAfterDamagedBuff(DamageContext ctx)
    {
        if(activeBuffs.Count == 0)
            yield break;
        
        // 只执行普通攻击
        if (ctx.type != DamageType.CommonAtkDamage)
            yield break;
        
        var sortedBuffs = SortBuffList(BuffType.AfterDamageBuff);
        
        foreach (var buff in sortedBuffs)
        {
            if (buff.source.behaviours == null) continue;
            
            foreach (var behaviour in buff.source.behaviours)
            {
                
                yield return behaviour.OnAfterDamaged(buff);
            }
            
            buff.remainingCharges--;
        }
    }
    
    /// <summary>
    /// 伤害修正
    /// </summary>
    /// <param name="ctx"></param>
    public void ModifyIncomingDamage(DamageContext ctx)
    {
        if (activeBuffs.Count == 0)
        {
            ctx.finalValue = ctx.baseValue;
            return;
        }
        
        if(ctx.isBlocked)
        {
            ctx.finalValue = 0;
            return;
        }

        
        var sortedBuffs = SortBuffList(BuffType.ModifyDamageBuff);
        if (sortedBuffs.Count == 0)
        {
            ctx.finalValue = ctx.baseValue;
            return;
        }
        
        Debug.Log("此处不通！此处不通！此处不通！此处不通！");
        
        foreach (var buff in sortedBuffs)
        {
            if (buff.source.behaviours == null) 
                continue;

            foreach (var behaviour in buff.source.behaviours)
            {
                behaviour.ModifyIncomingDamage(buff, ctx);
            }
        }
    }
    
    

    /// <summary>
    /// 能否进行行动
    /// </summary>
    public bool ActionBlockBuff()
    {
        bool blocked = false;
        
        if (activeBuffs.Count == 0)
            return true;

        foreach (var buff in activeBuffs)
        {
            if(buff.source.behaviours == null) 
                continue;

            foreach (var behaviour in buff.source.behaviours)
            {
                behaviour.OnCheckActionBlock(buff,ref blocked);

                if (blocked) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 是否背叛
    /// </summary>
    /// <returns></returns>
    public bool ActionBetrayBuff()
    {
        bool betrayed = false;
        
        if (activeBuffs.Count == 0)
            return true;

        foreach (var buff in activeBuffs)
        {
            if(buff.source.behaviours == null) 
                continue;

            foreach (var behaviour in buff.source.behaviours)
            {
                behaviour.OnCheckActionBetray(buff,ref betrayed);

                if (betrayed) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 是否行为限制
    /// </summary>
    /// <param name="isSilenced"></param>
    /// <param name="isDisarmed"></param>
    public void ActionRestriction(out bool isSilenced, out bool isDisarmed)
    {
        isSilenced = false;
        isDisarmed = false;

        if (activeBuffs.Count == 0)
            return;

        foreach (var buff in activeBuffs)
        {
            if (buff.source.behaviours == null) 
                continue;

            foreach (var behaviour in buff.source.behaviours)
            {
                behaviour.OnCheckActionRestriction(buff,ref isSilenced, ref isDisarmed);
            }
        }
    }


    public void EndOfTurnSettlement()
    {
        activeBuffs.RemoveAll(buff => {
            buff.remainingRounds--;
            return buff.remainingRounds <= 0;
        });
    }
    
    /// <summary>
    /// Utility
    /// </summary>
    /// <param name="buffType"></param>
    /// <returns></returns>
    private List<BuffInstance> SortBuffList(BuffType buffType)
    {
        var sortedBuffs = activeBuffs
            .Where(b => b.source.buffType == buffType && b.source.behaviours != null)
            .OrderByDescending(b => b.source.buffPriority)
            .ToList();
        
        return sortedBuffs;
    }
    
}
