using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
[CreateAssetMenu(fileName = "AttackSkillSO", menuName = "Skills/AttackSkillSO")]
public class AttackSkillSO : SkillBaseSO
{
    public AttackType attackType;
    
    public AttackTargetType attackTargetType;
    public override AttackTargetType GetAttackTargetType() => attackTargetType;
    
   // 调整起始位置
    public Vector3 projectileOffset = new Vector3(0, 1, 0);

    [Header("Rogue Settings")] 
    public int multiHitCount = 2;

    [SerializeField]
    private float arcShotPower = 1.5f;  // 飞行时间倍率，越大飞得越慢，越小越快
    [SerializeField]
    private float leapSlamPower = 1f; //特效，以及撞击的剧烈程度。
    [SerializeField]
    private float magicBoltPower = 1f;
    [SerializeField]
    private float multiHitPower = 1f;
    

    public override IEnumerator Activate(SoldierInBattle caster, List<SoldierInBattle> targets)
    {
        if (targets == null || targets.Count == 0)
            yield break;

        int count = Mathf.Min(targetCount, targets.Count);

        for (int i = 0; i < count; i++)
        {
            var target = targets[i];
            if (target == null) continue;

            int value = base.ValueCalculator(caster.att, caster.mag, caster.rage);

            var ctx = new DamageContext(value, DamageType.SkillDamage);

            switch (attackType)
            {
                case AttackType.ArcShot:
                    yield return ArcShotAttack(caster, target, ctx);
                    break;
                case AttackType.LeapSlam:
                    yield return LeapSlamAttack(caster, target, ctx);
                    break;
                case AttackType.MagicBolt:
                    yield return MagicBoltAttack(caster, target, ctx);
                    break;
                case AttackType.MultiHit:
                    yield return MultiHitAttack(caster, target, ctx);
                    break;
            }
            
            Debug.Log(extras+"extra");
            // Extra效果
            if (extras != null)
            {
                foreach (var e in extras)
                {
                    Debug.Log($"🧬【施加 Extra】{target.name} 被 {caster.name} 施加了 {e.name}");
                    yield return e.ExtraActivate(caster, target, value);
                }
                
            }
            
            
            // 施加Buff
            if (buffs != null)
            {
                var attributes = new SoldierAttributes(caster.maxHp, caster.hp, caster.att, caster.mag, caster.spe);
                
                foreach (var b in buffs)
                {
                    target.AddBuff(b,attributes);
                }
            }
            

        }
        
    }

    private IEnumerator MultiHitAttack(SoldierInBattle caster, SoldierInBattle target, DamageContext ctx)
    {
        var sprite =caster.GetComponent<SpriteRenderer>();
        var originPos=caster.transform.position;
        var originScale = caster.transform.localScale;

        var targetPos = target.transform.parent.position + new Vector3(0, 0, -0.1f);

        Vector3 chargeScale = originScale * 1.25f;

        Sequence seq = DOTween.Sequence();
        
       // 多段攻击：动作拆解
       //   1. 后撤步 
       Vector3 backStepPos = originPos + new Vector3(0.5f*multiHitPower, 0, 0);
       seq.Append(caster.transform.DOMove(backStepPos, 0.25f/multiHitPower)).SetEase(Ease.InBack);
       seq.AppendInterval(0.1f);
       
       //   2. 向前冲刺 前进一段距离后消失
       //      （残影慢慢消失）
       Vector3 disappearPos = originPos + new Vector3(-1.5f*multiHitPower, 0, 0);
       seq.Append(caster.transform.DOMove(disappearPos, 0.25f)).SetEase(Ease.OutExpo);
       seq.AppendCallback(() => sprite.enabled = false);
       
       //   3. 在敌人面前出现 向前刺击 n段 后消失
       //      （残影出现，攻击是向前冲刺速度由快到慢具有卡肉感，最后一段攻击打击感增强）
       //      -- 造成伤害执行yield return target.TakeDamage(damage)
       Vector3 appearPos = targetPos + new Vector3(1.4f*multiHitPower, 0, 0); 
       
       for (int i = 0; i < multiHitCount; i++)
       {
           seq.AppendCallback(() =>
           {
               caster.transform.position=appearPos;
               sprite.enabled = true;
           });
           
           //刺入
           //如果是最后一下 刺入更有力

           seq.Append(caster.transform.DOMove(targetPos, (i == multiHitCount - 1 ? 0.2f : 0.3f) / multiHitPower))
               .SetEase(i == multiHitCount - 1 ? Ease.OutExpo : Ease.OutQuad);

           seq.AppendCallback(() =>
           {
               target.StartCoroutine(target.TakeDamage(ctx));
           });
           
           //收刀
           seq.Append(caster.transform.DOMove(appearPos, 0.4f)).SetEase(Ease.OutSine);

       }
       
       seq.AppendCallback(() => sprite.enabled = false);
       
       //   4. 在初始位置前一段距离出现 慢慢滑行到初始地点 结束
       //      （残影出现）
       seq.AppendCallback(() =>
       {
           caster.transform.position = originPos + new Vector3(-0.5f, 0, 0);
           sprite.enabled = true;
       });
       
       seq.Append(caster.transform.DOMove(originPos, 0.2f)).SetEase(Ease.InOutSine);
       
       // 等待 Sequence 完成
       yield return seq.WaitForCompletion();

    }

    private IEnumerator ArcShotAttack(SoldierInBattle caster, SoldierInBattle target, DamageContext ctx)
    {
        float flyDuration = 0.5f;
        float projectileHeight = 1.5f;
        
        
        Vector3 startPos = caster.transform.position + projectileOffset;
        Vector3 targetPos = target.transform.position + projectileOffset;

        GameObject projectile = GameObject.Instantiate(hitEffectPrefab, startPos, Quaternion.identity);
        if (projectile == null) yield break;

        Transform proj = projectile.transform;
        proj.localScale = Vector3.one * 1.0f; // 固定缩放

        float duration = flyDuration * arcShotPower;
        float height = projectileHeight * 2f;
        float elapsed = 0f;
        Vector3 lastPos = startPos;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float yOffset = 4 * height * t * (1 - t);
            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y += yOffset;
            proj.position = pos;

            Vector3 moveDir = pos - lastPos;
            if (moveDir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                proj.rotation = Quaternion.Euler(0, 0, angle);
            }

            lastPos = pos;
            yield return null;
        }

        GameObject.Destroy(projectile);

        yield return target.TakeDamage(ctx);
    }
    
    private IEnumerator LeapSlamAttack(SoldierInBattle caster, SoldierInBattle target, DamageContext ctx)
{
    var originPos = caster.transform.position;
    var originScale = caster.transform.localScale;

    var targetSlot = target.transform.parent.position+new Vector3(0, 0, -0.1f);
    
    Vector3 chargeScale = originScale * 1.25f;

    Sequence seq = DOTween.Sequence();

    // 后撤蓄力（不受 slamPower 影响）
    Vector3 backStepPos = originPos + new Vector3(-0.3f, 0, 0);
    seq.Append(caster.transform.DOMove(backStepPos, 0.25f).SetEase(Ease.InOutSine));
    seq.Join(caster.transform.DOScale(chargeScale, 0.25f));
    seq.AppendInterval(0.1f);

    // 跳跃蓄势（不受 slamPower 影响）
    Vector3 jumpTargetPos = targetSlot + new Vector3(-0.25f, 0, 0);
    float jumpUpTime = 0.15f;
    float hoverTime = 0.1f;
    seq.Append(caster.transform.DOMove(jumpTargetPos + new Vector3(0f, 0.8f, 0), jumpUpTime).SetEase(Ease.OutCubic));
    seq.AppendInterval(hoverTime);

    // 砸下动作，力度影响距离和时间
    float baseSlamDistance = 0.3f; // 基础下砸距离
    float slamDistance = baseSlamDistance * leapSlamPower;

    float baseSlamDuration = 0.12f; // 基础时间
    float slamDuration = Mathf.Max(0.06f, baseSlamDuration / leapSlamPower); // 力度越大速度越快，限制最小时间

    Vector3 slamPos = jumpTargetPos - new Vector3(0f, slamDistance, 0f);
    seq.Append(caster.transform.DOMove(slamPos, slamDuration).SetEase(Ease.InQuad));

    // 命中特效和震屏，力度影响缩放和震屏强度
    seq.AppendCallback(() =>
    {
        if (extras == null)
        {
            Vector3 effectPos = target.transform.position + new Vector3(0f, 1f, -20f);
            GameObject effect = GameObject.Instantiate(hitEffectPrefab, effectPos, Quaternion.identity);
            if (effect != null)
            {
                Transform fx = effect.transform;
                fx.localScale = Vector3.one * 0.2f;

                float baseEffectScale = 1.8f;
                float maxEffectScale = baseEffectScale * leapSlamPower;

                fx.DOScale(maxEffectScale, 0.12f).SetEase(Ease.OutExpo)
                    .OnComplete(() =>
                    {
                        fx.DOScale(0f, 0.1f).SetEase(Ease.InBack)
                            .OnComplete(() => Destroy(effect));
                    });
            }
        }
        
        float baseShakeDuration = 0.3f;
        float baseShakeStrength = 0.4f;
        float shakeDuration = baseShakeDuration * Mathf.Clamp(leapSlamPower, 0.5f, 2f);
        float shakeStrength = baseShakeStrength * leapSlamPower;

        Camera.main.DOShakePosition(shakeDuration, shakeStrength, 20, 90);
        target.StartCoroutine(target.TakeDamage(ctx));
    });

    // 缓慢回位
    seq.AppendInterval(0.05f);
    seq.Append(caster.transform.DOMove(originPos, 0.45f).SetEase(Ease.InOutSine));
    seq.Join(caster.transform.DOLocalRotate(Vector3.zero, 0.2f));
    seq.Append(caster.transform.DOScale(originScale, 0.15f));

    yield return seq.WaitForCompletion();
}

    private IEnumerator MagicBoltAttack(SoldierInBattle caster, SoldierInBattle target, DamageContext ctx)
    {
        // ==== 打击感调节参数（magicPower 越大越猛） ====
        float prepareDuration = 0.2f / magicBoltPower;
        float thrustDuration = 0.1f / magicBoltPower;
        float magicFlyDuration = 0.5f / magicBoltPower;
        float recoilAngle = 20f * magicBoltPower; // 越猛角度越大

        // === 角色准备动作 ===
        Quaternion originRot = caster.transform.rotation;

        // 后仰
        caster.transform
            .DORotate(new Vector3(0, 0, -recoilAngle), prepareDuration / 2)
            .SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(prepareDuration / 2);

        // 前震
        caster.transform
            .DORotate(new Vector3(0, 0, recoilAngle * 1.5f), thrustDuration)
            .SetEase(Ease.InQuad);
        yield return new WaitForSeconds(thrustDuration);

        // 恢复原姿势
        caster.transform
            .DORotateQuaternion(originRot, 0.1f / magicBoltPower);

        // === 魔法飞行体 ===
        Vector3 startPos = caster.transform.position + projectileOffset;
        Vector3 targetPos = target.transform.position + projectileOffset;

        GameObject projectile = Instantiate(hitEffectPrefab, startPos, Quaternion.identity);
        if (projectile == null) yield break;

        Transform proj = projectile.transform;
        proj.localScale = Vector3.one;

        Vector3 direction = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        proj.rotation = Quaternion.Euler(0, 0, angle);

        // 飞行
        // float elapsed = 0f;
        // while (elapsed < magicFlyDuration)
        // {
        //     elapsed += Time.deltaTime;
        //     float t = Mathf.Clamp01(elapsed / magicFlyDuration);
        //     proj.position = Vector3.Lerp(startPos, targetPos, t);
        //     yield return null;
        // }
        
        // 飞行 - DOTween
        yield return proj.DOMove(targetPos, magicFlyDuration).SetEase(Ease.Linear).WaitForCompletion();

        Destroy(projectile);

        // === 伤害应用 ===
        yield return target.TakeDamage(ctx);
        

    }


}
