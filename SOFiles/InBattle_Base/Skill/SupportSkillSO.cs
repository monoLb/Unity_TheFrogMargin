using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SupportSkillSO", menuName = "Skills/SupportSkillSO")]
public class SupportSkillSO : SkillBaseSO
{
    public SupportType supportType;
 
    public SupportTargetType SupportTargetType;
    
    public FactionType factionType;

    public int swayCount = 2;
    
    public CauseType causeType;

    [Header("Value")] 
    public int BoostValue = 1;    
    public override SupportTargetType GetSupportTargetType() => SupportTargetType;
    public override FactionType GetSupportFactionType() => factionType;
    /// <summary>
    /// 动画力度
    /// </summary>
    [SerializeField] private float SupportPower = 1f;

    // 调整起始位置
    public Vector3 projectileOffset = new Vector3(0, 1, 0); 

    public override IEnumerator Activate(SoldierInBattle caster, List<SoldierInBattle> targets)
    {
        if (targets == null || targets.Count == 0)
            yield break;
        int count = Mathf.Min(targetCount, targets.Count);

        if (supportType == SupportType.AreaCircle)
        {
            yield return AreaCircleSupport(caster);
        }

        int value = 0;
        switch (causeType)
        {
            case CauseType.Heal:
            case CauseType.Revive:
                 value= ValueCalculator(caster.att, caster.mag, caster.rage);
                break;
            case CauseType.Boost:
            case CauseType.Weaken:
                value = BoostValue;
                break;
            case CauseType.Other:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        for (int i = 0; i < count; i++)
        {
            var target = targets[i];
            if (target == null) continue;
            
            switch (supportType)
            {
                case SupportType.SingleCircle:
                    yield return SingleCircleSupport(caster, target, value);
                    break;
                case SupportType.AreaCircle:
                    target.ReceiveCause(causeType,value);
                    break;
                case SupportType.Projectile:
                    yield return ProjectileSuppot(caster, target, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
            // //施加StateBuff
            // if (stateBuff != null)
            // {
            //     Debug.Log($"🧬【施加 Buff】{target.name} 被 {caster.name} 施加了 {stateBuff.name}");
            //     stateBuff.AddStateBuff(caster, target);
            // }
            //
            //
            // Debug.Log(extra+"extra");
            // // Extra效果
            // if (extra != null)
            // {
            //     Debug.Log($"🧬【施加 Extra】{target.name} 被 {caster.name} 施加了 {extra.name}");
            //     yield return extra.ExtraActivate(caster, target, value);
            // }
            //
            //
            // //施加Valuebuff
            // if(ValueBuff!=null)
            // {
            //     Debug.Log($"🧬【施加 Debuff】{target.name} 被 {caster.name} 施加了 {ValueBuff.name}");
            //     ValueBuff.AddValueBuff(caster, target);
            // }
    

            
        }
        
    }

    /// <summary>
    /// 投射类辅助技能 类似attack skill的 MagicAttack
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private IEnumerator ProjectileSuppot(SoldierInBattle caster, SoldierInBattle target, int value)
{
    float prepareDuration = 0.8f / SupportPower;        // 下沉时长
    float projectileFlyDuration = 0.67f / SupportPower; // 飞行速度
    float pauseDuration = 0.2f;

    float downRange = 0.1f * SupportPower;              // 下沉距离
    float startStrength = 0.2f;  // 初始震幅（大）
    float endStrength = 0.02f;    // 结束震幅（小）
    int shakeVibrato = 10;                              // 高频震动次数（越大震动越快）

    Transform casterTf = caster.transform;
    Vector3 originPos = casterTf.position;
    Vector3 downPos = originPos + Vector3.down * downRange;
    
    Sequence seq = DOTween.Sequence();

// 下沉
    seq.Append(
        casterTf.DOMoveY(downPos.y, prepareDuration)
            .SetEase(Ease.OutCubic)
    );

// 震动（幅度大→小，频率快，且不会漂移）
    seq.Join(
        DOTween.To(
            () => 0f,
            v =>
            {
                float t = v / prepareDuration;

                // 当前震幅（线性衰减）
                float strength = Mathf.Lerp(startStrength, endStrength, t);

                // 高频震动：使用 sin，而不是随机，保证稳定又自然
                float freq = shakeVibrato * 6.28f; // 震动次数映射成频率

                // 生成左右震动（幅度 = strength）
                float x = Mathf.Sin(Time.time * freq) * strength;

                // 应用震动（始终围绕原位置，不会漂移）
                casterTf.position = new Vector3(
                    originPos.x + x,
                    casterTf.position.y,
                    originPos.z
                );
            },
            prepareDuration,
            prepareDuration
        )
    );

    yield return seq.WaitForCompletion();

// 恢复精确位置
    casterTf.position = downPos;


    yield return seq.WaitForCompletion();
    
    // 2️⃣ 停顿 → 发射
    yield return new WaitForSeconds(pauseDuration);

    // 飞行物
    Vector3 startPos = casterTf.position + projectileOffset;
    Vector3 targetPos = target.transform.position + projectileOffset;

    GameObject projectileObj = Instantiate(hitEffectPrefab, startPos, Quaternion.identity);
    if (!projectileObj) yield break;

    Transform proj = projectileObj.transform;
    proj.localScale = Vector3.one;

    Vector3 dir = (targetPos - startPos).normalized;
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    proj.rotation = Quaternion.Euler(0, 0, angle);

    Tweener fly = proj.DOMove(targetPos, projectileFlyDuration).SetEase(Ease.InQuad);
    yield return fly.WaitForCompletion();

    Destroy(projectileObj);

    // 效果命中
    target.ReceiveCause(causeType, value);
    
    // 3️⃣ 回到原位
    yield return casterTf.DOMove(originPos, 0.25f / SupportPower).SetEase(Ease.OutQuad).WaitForCompletion();
}


    private IEnumerator AreaCircleSupport(SoldierInBattle caster)
    {
        Transform casterTf = caster.transform;
        Vector3 originPos = casterTf.position;
        Vector3 originRot = casterTf.eulerAngles;

        float riseHeight = 0.8f;
        float riseDuration = 0.45f;
        float holdDuration = 0.25f;
        float fallDuration = 0.35f;

        // 上仰角度（Z轴向前仰）
        float tiltAngle = 10f;

        // 创建法阵
        GameObject circle = Instantiate(hitEffectPrefab, originPos, Quaternion.identity);
        circle.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        // --- ① 法阵出现 ---
        seq.Append(circle.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack));

        // --- ② 人物升空 + 上仰 ---
        Vector3 risePos = originPos + Vector3.up * riseHeight;

        seq.Append(
            casterTf.DOMove(risePos, riseDuration)
                .SetEase(Ease.OutCubic)
        );

        seq.Join(
            casterTf.DORotate(
                new Vector3(originRot.x, originRot.y, tiltAngle),
                riseDuration * 0.8f,
                RotateMode.Fast
            ).SetEase(Ease.OutSine)
        );

        // --- ③ 高空停顿 ---
        seq.AppendInterval(holdDuration);

        // --- ④ 缓缓恢复直立 + 缓缓下降 ---
        seq.Append(
            casterTf.DOMove(originPos, fallDuration)
                .SetEase(Ease.InCubic)
        );

        seq.Join(
            casterTf.DORotate(
                originRot,
                fallDuration * 0.7f,
                RotateMode.Fast
            ).SetEase(Ease.InOutSine)
        );

        // --- ⑤ 落地震动 ---
        seq.Append(
            casterTf.DOShakePosition(0.15f, strength: 0.07f, vibrato: 10)
        );

        // --- ⑥ 法阵淡出 ---
        seq.Join(
            circle.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack)
        );
        seq.Join(
            circle.GetComponent<SpriteRenderer>().DOFade(0f, 0.2f)
        );

        seq.OnComplete(() =>
        {
            Destroy(circle);
        });

        yield return seq.WaitForCompletion();
    }



    private IEnumerator SingleCircleSupport(SoldierInBattle caster, SoldierInBattle target, int value)
    {
        // === 施法者悬浮摇晃动画，力度和速度随 SupportPower 调节 ===
        Transform casterTf = caster.transform;
        Vector3 casterStartPos = casterTf.position;

        float casterUpDistance = 0.3f * SupportPower; // 上浮距离放大
        float casterDownDistance = 0.2f * SupportPower; // 下沉距离放大
        float casterUpDuration = 0.2f / SupportPower; // 动画时间缩放，力度大动作快
        float casterDownDuration = 0.2f / SupportPower;
        float casterReturnDuration = 0.2f / SupportPower;

        Sequence casterSeq = DOTween.Sequence();
        casterSeq.Append(casterTf.DOMoveY(casterStartPos.y + casterUpDistance, casterUpDuration).SetEase(Ease.OutQuad));
        casterSeq.Append(casterTf.DOMoveY(casterStartPos.y - casterDownDistance, casterDownDuration)
            .SetEase(Ease.InOutQuad));
        casterSeq.Append(casterTf.DOMoveY(casterStartPos.y, casterReturnDuration).SetEase(Ease.OutQuad));
        yield return casterSeq.WaitForCompletion();

        // === 遍历每个目标，播放浮动和特效，力度同样调整 ===

        Transform targetTf = target.transform;
        Vector3 originalPos = targetTf.position;

        float targetUpDistance = 0.2f * SupportPower;
        float targetUpDuration = 0.15f / SupportPower;
        float targetDownDuration = 0.15f / SupportPower;

        Sequence targetSeq = DOTween.Sequence();
        targetSeq.Append(targetTf.DOMoveY(originalPos.y + targetUpDistance, targetUpDuration).SetEase(Ease.OutQuad));
        targetSeq.Append(targetTf.DOMoveY(originalPos.y, targetDownDuration).SetEase(Ease.InQuad));

        if (hitEffectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(hitEffectPrefab, targetTf);
            effect.transform.localPosition = new Vector3(0, 1.1f, 0);
            effect.transform.localRotation = Quaternion.Euler(-26f, 0f, 0f);
            effect.transform.localScale = new Vector3(5f, 5f, 5f) * (1.5f * SupportPower);

            // DOTween 延时销毁
            float effectDuration = 1.0f; // 你可以根据特效动画时长调整

            effect.transform.DOScale(Vector3.zero, effectDuration).SetEase(Ease.InBack)
                .OnComplete(() => GameObject.Destroy(effect));
        }


        yield return targetSeq.WaitForCompletion();

        target.ReceiveCause(causeType, value);
        Debug.Log($"💚【治疗】{target.name} 恢复 {value} 点生命"); 


        yield return new WaitForSeconds(0.2f / SupportPower);
    }
    
    

}
