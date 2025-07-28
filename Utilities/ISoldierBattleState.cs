using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoldierBattleState
{
    IEnumerator Enter(SoldierInBattle soldier);
}
public class IdleState : ISoldierBattleState
{
    public IEnumerator Enter(SoldierInBattle soldier)
    {
        //soldier.PlayIdleAnimation();
        yield break;
    }
}

public class StartDebuffState : ISoldierBattleState
{
    public IEnumerator Enter(SoldierInBattle soldier)
    {
        yield return soldier.ApplyStartOfTurnDamageBuffs();
        //执行所有debuff
    }
}


public class ActionState : ISoldierBattleState
{
    public IEnumerator Enter(SoldierInBattle soldier)
    {
        if (!soldier.ApplyActionBlockBuffs()) //返回是否被阻止{
        {
            // 眩晕动画

            yield return soldier.PlayStunAnimation();
            
            yield break;
        }
            

        bool isSilenced, isDisarmed;
        soldier.ApplyActionRestrictBuffs(out isSilenced, out isDisarmed);

        if (soldier.CanCastSkill() && !isSilenced)
        {

            yield return soldier.CastSkill();
            soldier.rage = 0;
        }
        else if (!isDisarmed)
        {
            yield return soldier.CommonAttackWithAnimation();
        }
        else
        {
            // 还在就在不能攻击和释放技能
            // todo 播放一个疑惑动画
            Debug.Log($"{soldier.name} 无法攻击或施法！");
            yield return new WaitForSeconds(0.4f);
        }
    }
}

public class EndBuffState : ISoldierBattleState
{
    public IEnumerator Enter(SoldierInBattle soldier)
    {
        yield return soldier.ApplyEndOfTurnBuffs();
        // ❌ 不要再切换状态！
        // ✅ 等待外部 RunFullTurnRoutine 来切换 TurnEndState

        //执行所有积极buff
    }
}


public class TurnEndState : ISoldierBattleState
{
    public IEnumerator Enter(SoldierInBattle soldier)
    {
        soldier.buffSystem.EndOfTurnSettlement();

        yield return new WaitForSeconds(0.2f); // 可添加特效或清理工作
    }
}


