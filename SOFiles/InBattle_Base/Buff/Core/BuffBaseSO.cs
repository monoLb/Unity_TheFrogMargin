using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 模板类 组合搭配的Buff基类
/// </summary>
[CreateAssetMenu(menuName = "Buff/BuffBaseSO")]
public class BuffBaseSO : ScriptableObject
{
    public string buffName;
    public Sprite buffIcon;
    
    public BuffType buffType;

    [Header("Setting")] 
    public int buffPriority = 0; // 优先级
    public int duration; // -1 为不再buff阶段自动-1
    public int triggerTimes; // 触发次数
    
    
    public bool stackAble;
    public int maxStack = -1;

    public float procChance = 0.50f;
    
    public List<BuffBehaviourSO> behaviours;

}
