using UnityEngine;

public enum QualityType
{
    Green,
    Blue,
    Purple,
    Red,
    Gold
}

public enum ElementType
{
    Metal,
    Wood,
    Water,
    Fire,
    Earth
}

public enum EventStateType
{
    Undiscovered,
    Active,
    Completed,
}

public enum FactionType
{
    Friend,
    Enemy
}

public enum FormationLineType
{
    Frontline = 0,  // 最靠前
    Backline  = 1  // 最靠后
}

public enum BattleState
{
    Start,
    Combat,
    End
}

public enum SkillType
{
    Attack,
    Support,
}

public enum AttackType
{
    ArcShot,
    LeapSlam,
    MagicBolt,
    MultiHit
}

public enum SupportType
{
    SingleCircle,
    AreaCircle,
    Projectile
}

public enum CauseType
{
    Heal,       // 回复生命
    Boost,       // 属性强化
    Weaken,     // 属性削弱
    Revive,      // 复活
    Other,
}

public enum AttackTargetType
{
    Default,     //等于默认寻敌 
    Random,     //随机若干个敌人 
    Row,        //根据默认敌人的那一列 
    Line,       //根据默认敌人的那一行 
    Front,      //攻击所有前排敌人 
    Back,       //攻击所有后排敌人 
    MaxPower,        //攻击攻击力+魔法最高
    MinHp,        // 攻击血量最少的敌人 
    MaxHp,
    All         // 全体
}

public enum SupportTargetType
{
    Random, //随机目标
    MaxHp, //血量最高
    MinHp, //血量最低
    MaxPower, //攻击力 + 魔法最高的敌人
    Front, //当前前排，无则随机前排
    Back, //当前后排，无则随机后排
    All
}

/// <summary>
/// Buff 类型标识
/// 用途：分类、净化、免疫判断
/// </summary>

public enum BuffType
{
    TurnStartBuff,
    TurnEndBuff,
    ActionBlockBuff,
    ActionBetrayBuff,
    ActionRestrictBuff,
    AfterDamageBuff,
    BeforeDamageBuff,
    ModifyDamageBuff
}



public enum NegativeBuffType
{
    Poison,         // 中毒（毒伤）
    Burn,           // 着火（火伤）
    Bleed,          // 流血（物理持续伤害）
    Stun,           // 眩晕（无法行动）
    Sleep,          // 睡眠（受击醒来）
    Charm,          // 魅惑（攻击友军）
    Silence,        // 沉默（不能使用技能）
    Disarm,         // 缴械（不能普通攻击）
    Blind,          // 致盲（50%概率无法命中）
    Vulnerable,     // 易伤（受伤增加）
}



public enum ResultType
{
    Currency,       // 货币
    Soldier,      // 角色
    QuestScroll,    // 任务书
    StoryFragment   // 剧情碎片
} 

public enum MapThemeType
{
    // 基础阶段
    FrontWasteland,       // 屋前荒地
    
    // 第一阶段 chapter =2   --7
    BrokenAbbey,          // 破修道院
    LanternTown,          // 风灯镇
    Wolfwood,             // 狼头林
    BanditFort,           // 盗贼堡垒
    Starwell,             // 星井村
    MurlocSwamp,          // 鱼人水潭
    MistMarket,           // 雾市

    // 第二阶段 chapter =3   --8
    BlasphemeHall,        // 渎神法堂
    BloodChapel,          // 血色主教厅
    Ashforge,             // 灰炉镇
    HereticVillage,       // 异端村社
    DreamWell,            // 梦井
    Frostvale,            // 白霜谷
    ScholarRuins,         // 学者塔遗迹
    TruthTheatre,         // 真理剧场

    // 第三阶段 chapter =2   --5
    SacredCourt,          // 圣心庭
    GallowsSquare,        // 绞刑广场
    LanternStreet,        // 槐灯街
    BoneSpring,           // 龙骨水坛
    PlumPalace,           // 青梅幽宫
    
    // 皇宫阶段 
    ThroneHall,           // 皇座殿

    // 终焉阶段
    DreamAbyss,           // 终焉梦渊 无Key
    ReversedEye,          // 逆梦之眼 有Key

    // 支线事件
    AshforgeFall,         // 灰炉镇陷落
    LanternFall,          // 风灯镇沦陷
    FrostvaleHold         // 白霜谷残守
}

