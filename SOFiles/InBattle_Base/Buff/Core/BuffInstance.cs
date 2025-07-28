using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 战斗中的执行
/// </summary>
[System.Serializable]
public class BuffInstance
{
    public BuffBaseSO source;
    
    public int remainingRounds;
    /// <summary>
    /// 次数，流血后每次被攻击都会受到额外伤害
    /// </summary>
    public int remainingCharges;
    
    public SoldierInBattle owner;
    public int stackCount;

    public SoldierAttributes casterAttributes;
    
    public BuffInstance(BuffBaseSO source, SoldierInBattle owner,int stackCount,SoldierAttributes casterAttributes)
    {
        this.source = source;
        this.owner = owner;
        remainingRounds = source.duration;
        remainingCharges = source.triggerTimes;
        this.stackCount = stackCount;
        this.casterAttributes = casterAttributes;
    }

    public bool IsExpired => source.duration >= 0 && remainingRounds <= 0 || stackCount <= 0;

}
  
[System.Serializable]
public class SoldierAttributes
{
    public int maxHp;
    public int hp;
    public int att;
    public int mag;
    public int spe;

    public SoldierAttributes(int maxHp, int hp, int att, int mag, int spe)
    {
        this.maxHp = maxHp;
        this.hp = hp;
        this.att = att;
        this.mag = mag;
        this.spe = spe;
    }
}