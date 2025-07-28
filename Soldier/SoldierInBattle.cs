using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

[Serializable]
public class SoldierInBattle : MonoBehaviour
{
    public SoldierInstance soldierInstance;
    public SoldierBattleStateMachine StateMachine;
    public FactionType factionType;
    private FactionType _originalFactionType;
    
    
    public int maxHp;
    public int hp;
    public int att;
    public int mag;
    public int spe;

    public int maxRage;
    public int rage;
    public bool isAttacking;
    public bool isDead;

    public float attackForce = 2f;
    
    [Header("System")]
    public BuffSystem buffSystem;

    [Header("Prefabs")] 
    public GameObject _damageTextPrefab;
    public GameObject _buffPrefab;

    [Header("Transforms")] 
    public Transform buffGroup;

    [SerializeField]
    private List<BuffInstance> activeBuffs = new();
    
    // 管理GameObjects
    public List<GameObject> buffObjects;
    
    public Slider healthSlider;
    public Image healthFill;
    //AC3232 = 172 50 50
    [SerializeField] private Color healthFillColor = new Color(172 / 255f, 50 / 255f, 50 / 255f);
    
    public Image rageLeft;
    public Image rageRight;
    
    
    // TODO 调度资源更好的方法
    public Sprite rageImage_Empty;
    public Sprite rageImage_Half;
    public Sprite rageImage_All;

    private Tween _angryLoopTween;
    private bool _isAngryPlaying = false;

    private void Awake()
    {
        buffSystem = new BuffSystem(this, activeBuffs);
    }
    
   
    void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
    }
    void SetHealth(int health)
    {
        healthSlider.value = health;
        
        float ratio = (float)hp / maxHp;
        
        if (ratio <= 0.1f)
        {
            healthFill.color=Color.white;
        }
        else 
        {
            healthFill.color=healthFillColor;
        }
    }

    void GetRageByAttack()
    {
        rage += 50;
        RefreshRageUI();
    }

    void GetRageByHurt()
    {
        rage += 25;
        RefreshRageUI();
    }

    void RefreshRageUI()                  
    {   
        if (rage < 100)
        {
            SetOutLine(false);
            UpdateAngryEffect();
            
            if (rage == 0)
            {
                rageLeft.sprite = rageImage_Empty;
                rageRight.sprite = rageImage_Empty;
            }

            else if (rage == 25)
            {
                rageLeft.sprite = rageImage_Half;
                rageRight.sprite = rageImage_Empty;
            }

            else if (rage == 50)
            {
                rageLeft.sprite = rageImage_All;
                rageRight.sprite = rageImage_Empty;
            }

            else if (rage == 75)
            {
                rageLeft.sprite = rageImage_All;
                rageRight.sprite = rageImage_Half;
            }

        }
        
        else if (rage >= 100)
        {
            rageLeft.sprite = rageImage_All;
            rageRight.sprite = rageImage_All;
            
            SetOutLine(true);
            UpdateAngryEffect();
        }
    }
    
    private void UpdateAngryEffect()
    {
        if (rage >= 100)
        {
            if (_isAngryPlaying) return;

            _isAngryPlaying = true;

            // ✅ 先终止之前的动画，防止叠加
            if (_angryLoopTween != null) _angryLoopTween.Kill();
            DOTween.Kill("AngryColor_" + GetInstanceID());

            transform.localScale = Vector3.one; // 重置一下大小以防残留影响

            // 播放持续的怒气动画：循环放大/缩小
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(1.1f, 0.3f));
            seq.Append(transform.DOScale(1f, 0.3f));
            seq.SetLoops(-1, LoopType.Yoyo);
            _angryLoopTween = seq;

            // 红色闪烁
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.DOColor(new Color(1f, 0.6f, 0.6f), 0.2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetId("AngryColor_" + GetInstanceID());
        }

        else
        {
            if (!_isAngryPlaying) return;

            _isAngryPlaying = false;

            // 停止动画还原状态
            if (_angryLoopTween != null) _angryLoopTween.Kill();
            transform.localScale = Vector3.one;

            // 恢复颜色
            DOTween.Kill("AngryColor_" + GetInstanceID());
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void Init(SoldierInstance data, FactionType factionType)
    {
        soldierInstance = data;
        this.factionType = factionType;
        _originalFactionType = factionType;
        
        maxHp = data.health;
        att = data.attack;
        mag = data.magic;
        spe = data.speed;

        hp = maxHp;
        SetMaxHealth(maxHp);
        
        //优化角色血条UI
        RefreshCanvas();
    }

    private void SetOutLine(bool isShowed)
    {
        SpriteRenderer sr= GetComponent<SpriteRenderer>();
        MaterialPropertyBlock mpb=new MaterialPropertyBlock();
        
        // 获取当前pb
        sr.GetPropertyBlock(mpb);
        
        // 是否开启
        mpb.SetInt("_ShowOutLine", isShowed ? 1 : 0);
        
        // 提交
        sr.SetPropertyBlock(mpb);
    }

    private void RefreshCanvas()
    {
        var canvasTransform = transform.GetChild(0);

        Vector3[] positions =
        {
            new Vector3(0f, 6.97f, -5f),   // index 0~2
            new Vector3(0f, 6.2f,  -3.61f),// index 3~5
            new Vector3(0f, 6.3f,  -4.56f) // index 6~8
        };

        int row = soldierInstance.formationIndex / 3;
        if (row >= 0 && row < positions.Length)
            canvasTransform.localPosition = positions[row];
        else
            Debug.LogWarning("Invalid formationIndex: " + soldierInstance.formationIndex);
    }

    public void PlayIdleAnimation()
    {
        // 在此播放 idle 动画
    }

    public bool CanCastSkill() => rage >= maxRage;

    /// <summary>
    /// 整个角色的完整回合：由 BattleSystem 调用
    /// </summary>
    public IEnumerator RunFullTurnRoutine(Action onFinished)
    {
        // 判定是否结束战斗
        BattleSystem.Instance.CheckBattleEnd();
        
        if (isDead || this == null)
        {
            onFinished?.Invoke(); // 通知战斗系统继续
            yield break;
        }

        yield return StateMachine.SetStateAndWait(new StartDebuffState(), this);
        if (isDead || this == null)
        {
            onFinished?.Invoke();
            yield break;
        }

        yield return StateMachine.SetStateAndWait(new ActionState(), this);
        if (isDead || this == null)
        {
            onFinished?.Invoke();
            yield break;
        }

        yield return StateMachine.SetStateAndWait(new EndBuffState(), this);
        if (isDead || this == null)
        {
            onFinished?.Invoke();
            yield break;
        }

        yield return StateMachine.SetStateAndWait(new TurnEndState(), this);
        
        onFinished?.Invoke(); // 正常完成回合
    }



    /// <summary>
    /// 普通攻击流程（由 AttackState 调用）
    /// </summary>
    public IEnumerator CommonAttackWithAnimation()
    {
        if (isDead || this == null) yield break;

        var target = GetDefaultAttackTarget(soldierInstance.formationIndex, factionType);
        if (target == null)
        {
            yield break;
        }

        yield return AnimateAttack(target);
    }

    
    public IEnumerator TakeDamage(DamageContext ctx)
    {
        // 安全判断
        if (isDead || !this) yield break;
        
        // 判断能否造成此伤害 执行顺序 BUFF、（后续：被动、命定卡）
        yield return buffSystem.OnBeforeDamageBuff(ctx);
            //播放击穿护盾等动画
            
        // 伤害修正：增伤buff，削弱buff等
        buffSystem.ModifyIncomingDamage(ctx);
        
        // 实际伤害造成 动画 弹出数值
        yield return TakeDamageFeedBack(ctx.finalValue);
        hp -= ctx.finalValue;

        // 执行伤害后行为
        yield return buffSystem.OnAfterDamagedBuff(ctx);

        // 死亡判断
        if (hp <= 0)
        {
            Die();
        }

    }
    
    public void ReceiveCause(CauseType causeType, int value)
    {
        if(!this) return;
        if(causeType != CauseType.Revive && isDead) return;

        switch(causeType)
        {
            case CauseType.Heal:
                hp = Mathf.Min(hp + value, maxHp);
                SetHealth(hp);
                ShowFloatingValueText(value, false);
                break;
            case CauseType.Boost:
                //提升具体数值，生成UI预制体
                break;
            case CauseType.Weaken:
                break;
            case CauseType.Revive:
                break;
            case CauseType.Other:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(causeType), causeType, null);
        }
    }

    
    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"{name} 死亡");

        PlayDeathAnimation();

        BattleSystem.Instance.battleList.Remove(this);

        // 立刻移除对应的UI
        BattleSystem.Instance.RemovePreparationUIFor(this);
    }
    

    void PlayDeathAnimation()
    {
        Sequence deathSeq = DOTween.Sequence();
        deathSeq.Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));
        deathSeq.Join(GetComponent<SpriteRenderer>().DOFade(0, 0.5f));
        deathSeq.OnComplete(() =>
        {
            Destroy(gameObject); // ✅ 最终移除角色
        });
    }


    private IEnumerator TakeDamageFeedBack(int damage)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.white;

            Sequence seq = DOTween.Sequence();

            seq.Append(transform.DOLocalMoveY(1f, 0.2f).SetEase(Ease.OutQuad));
            seq.Append(transform.DOLocalMoveY(0f, 0.2f).SetEase(Ease.InQuad));
        
            // 左右晃动
            seq.Append(transform.DOShakePosition(0.8f, new Vector3(1.0f, 0.3f, 0), 15, 45));

            yield return seq.WaitForCompletion();

            sr.color = originalColor;
        }

        ShowFloatingValueText(damage,true);
        SetHealth(hp);
        yield return null;
    }

    private void ShowFloatingValueText(int value, bool isDamage)
    {
        GameObject prefab = _damageTextPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("未找到 FloatingText 预制体！");
            return;
        }
        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;
        GameObject textObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        textObj.transform.SetParent(this.transform);

        var text = textObj.GetComponent<TextMeshPro>();
        text.fontSize = 2f;

        if (isDamage)
        {
            text.text = $"-{value}";
            text.color = Color.red;

            Sequence seq = DOTween.Sequence();
            seq.Append(textObj.transform.DOMoveY(spawnPos.y + 1f, 0.8f).SetEase(Ease.OutQuad));
            textObj.transform.DOLocalMoveX(spawnPos.x + 0.02f, 0.25f).SetLoops(4, LoopType.Yoyo);            seq.Join(textObj.transform.DOScale(0.7f, 0.8f));
            seq.Join(textObj.GetComponent<CanvasGroup>().DOFade(0, 0.8f));
            seq.OnComplete(() => Destroy(textObj));
        }
        else
        {
            text.text = $"+{value}";
            text.color = Color.green;

            Sequence seq = DOTween.Sequence();
            seq.Append(textObj.transform.DOMoveY(spawnPos.y + 1f, 1f).SetEase(Ease.OutQuad));
            seq.Join(textObj.transform.DOScale(1.3f, 1f));
            seq.Join(textObj.GetComponent<CanvasGroup>().DOFade(0, 1f));
            seq.OnComplete(() => Destroy(textObj));
        }

        // 朝向摄像机
        textObj.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    
    
    private IEnumerator AnimateAttack(SoldierInBattle target)
    {
        isAttacking = true;

        // =============================
        // 0. 保存原始状态
        // =============================
        var originLocalPos = transform.localPosition;
        var originScale = transform.localScale;

        var targetSlot = target.transform.parent.position + new Vector3(0f, 0f, -0.1f);

        float selfScaleFactor = GetRowScaleFactor(soldierInstance.formationIndex + 1);
        float targetScaleFactor = GetRowScaleFactor(target.soldierInstance.formationIndex + 1);

        bool isCrossRow = !Mathf.Approximately(selfScaleFactor, targetScaleFactor);
        float scaleRatio = targetScaleFactor / selfScaleFactor;

        Vector3 targetVisualScale = originScale * scaleRatio;
        Vector3 chargeScale = originScale * 1.1f;

        // =============================
        // 1. 攻击前进动画（到命中）
        // =============================
        Sequence attackSeq = DOTween.Sequence();

        attackSeq.Append(transform.DOScale(chargeScale, 0.15f).SetEase(Ease.OutQuad));
        attackSeq.Append(transform.DOScale(originScale, 0.1f));
        attackSeq.Append(transform.DOLocalRotate(new Vector3(0, 0, -30f), 0.15f));

        float jumpDuration = 0.4f;
        Vector3 targetLocalPos = transform.parent.InverseTransformPoint(targetSlot);

        var approachPos = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y + 2f,
            Mathf.Lerp(transform.localPosition.z, targetLocalPos.z, 0.25f)
        );

        float total = jumpDuration / attackForce;
        float t1 = total * 0.5f;
        float t2 = total * 0.8f;

        attackSeq.Append(
            DOTween.Sequence()
                .Append(transform.DOLocalMove(approachPos, t1).SetEase(Ease.InOutSine))
                .Append(transform.DOLocalMove(targetLocalPos, t2).SetEase(Ease.OutCubic))
        );

        if (isCrossRow)
        {
            attackSeq.Join(transform.DOScale(targetVisualScale, jumpDuration).SetEase(Ease.InOutSine));
        }

        // =============================
        // 2. 命中点：触发伤害（不等待）
        // =============================
        bool damageFinished = false;

        attackSeq.AppendCallback(() =>
        {
            Camera.main.DOShakePosition(0.2f, 0.3f, 20, 90);

            var ctx = new DamageContext(this.att, DamageType.CommonAtkDamage);

            // ❗关键：伤害逻辑并行执行
            StartCoroutine(DealDamageRoutine(target, ctx, () =>
            {
                damageFinished = true;
            }));
        });

        yield return attackSeq.WaitForCompletion();

        // =============================
        // 3. 回位动画（立刻播放）
        // =============================
        Sequence returnSeq = DOTween.Sequence();

        Vector3 returnLocalPos = new Vector3(originLocalPos.x, originLocalPos.y, -0.1f);

        returnSeq.Append(transform.DOLocalMove(returnLocalPos, 0.6f).SetEase(Ease.InOutSine));
        returnSeq.Join(transform.DOLocalRotate(Vector3.zero, 0.3f));
        returnSeq.Append(transform.DOScale(originScale, 0.15f));
        returnSeq.AppendCallback(() =>
        {
            transform.localPosition = originLocalPos;
        });

        yield return returnSeq.WaitForCompletion();

        // =============================
        // 4. 等待伤害链（流血 / 死亡）完成
        // =============================
        yield return new WaitUntil(() => damageFinished);

        GetRageByAttack();
        isAttacking = false;
    }
    private IEnumerator DealDamageRoutine(
        SoldierInBattle target,
        DamageContext ctx,
        System.Action onFinished)
    {
        yield return target.TakeDamage(ctx);
        onFinished?.Invoke();
    }


    public float GetRowScaleFactor(int formationIndex)
    {
        if (formationIndex >= 1 && formationIndex <= 3) return 1.2f;  // 前排
        if (formationIndex >= 4 && formationIndex <= 6) return 1.1f;  // 中排
        return 1.0f;                                                  // 后排
    }


    private SoldierInBattle GetDefaultAttackTarget(int index, FactionType selfFaction)
    {

        int[] currentRow = GetTargetRow(index + 1); // formationIndex是0-8，棋盘是1-9

        foreach (var i in currentRow)
        {
            string nameTag = $"INDEX_{i}";
            var target = BattleSystem.Instance.battleList.FirstOrDefault(s =>
                s.name.Contains(nameTag) &&
                !s.isDead &&
                s.factionType != selfFaction);
            if (target != null)
                return target;
        }

        // 没找到 → 随机一排
        int[] otherRow = GetRandomOtherRow(index + 1);
        foreach (var i in otherRow)
        {
            string nameTag = $"INDEX_{i}";
            var target = BattleSystem.Instance.battleList.FirstOrDefault(s =>
                s.name.Contains(nameTag) &&
                !s.isDead &&
                s.factionType != selfFaction);
            if (target != null)
                return target;
        }

        // 再没找到 → 剩下那排
        int[] lastRow = GetRemainingRow(currentRow, otherRow);
        foreach (var i in lastRow)
        {
            string nameTag = $"INDEX_{i}";
            var target = BattleSystem.Instance.battleList.FirstOrDefault(s =>
                s.name.Contains(nameTag) &&
                !s.isDead &&
                s.factionType != selfFaction);
            if (target != null)
                return target;
        }

        // 所有都没找到
        return null;
    }

    int[] GetTargetRow(int index)
    {
        if (index >= 1 && index <= 3)
            return new int[] { 3, 2, 1 }; // 前排
        else if (index >= 4 && index <= 6)
            return new int[] { 6, 5, 4 }; // 中排
        else if (index >= 7 && index <= 9)
            return new int[] { 9, 8, 7 }; // 后排
        else
            return new int[0]; // 越界
    }

    int[] GetRandomOtherRow(int index)
    {
        int[][] rows = new int[][]
        {
            new int[] { 3, 2, 1 }, // 前排
            new int[] { 6, 5, 4 }, // 中排
            new int[] { 9, 8, 7 }  // 后排
        };

        int primaryIndex = -1;
        if (index >= 1 && index <= 3) primaryIndex = 0;
        else if (index >= 4 && index <= 6) primaryIndex = 1;
        else if (index >= 7 && index <= 9) primaryIndex = 2;

        if (primaryIndex == -1) return new int[0];

        // 其他两个排
        List<int[]> otherRows = new List<int[]>();
        for (int i = 0; i < 3; i++)
        {
            if (i != primaryIndex)
                otherRows.Add(rows[i]);
        }

        return otherRows[Random.Range(0, otherRows.Count)];
    }

    int[] GetRemainingRow(int[] row1, int[] row2)
    {
        List<int[]> allRows = new List<int[]>
        {
            new int[] { 3, 2, 1 },
            new int[] { 6, 5, 4 },
            new int[] { 9, 8, 7 }
        };

        return allRows.FirstOrDefault(row =>
            !row.SequenceEqual(row1) && !row.SequenceEqual(row2));
    }
    

    public IEnumerator CastSkill()
    {
        // 设置为不可控（可选）
        isAttacking = true;

        var originPos = transform.position;
        var originRot = transform.rotation;
        
        
        yield return StartCoroutine(
            soldierInstance.soldierData.soldier_Skill.Activate(
                this,
                soldierInstance.soldierData.soldier_Skill.skillType == SkillType.Attack
                    ? GetAttackSkillTargets(
                        soldierInstance.soldierData.soldier_Skill.targetCount,
                        soldierInstance.soldierData.soldier_Skill.GetAttackTargetType()
                    )
                    : GetSupportSkillTargets(
                        soldierInstance.soldierData.soldier_Skill.targetCount,
                        soldierInstance.soldierData.soldier_Skill.GetSupportTargetType(),
                        soldierInstance.soldierData.soldier_Skill.GetSupportFactionType()
                    )
            )
        );

        
        // 技能释放完毕，清空怒气
        rage = 0;
        RefreshRageUI(); // 如有 UI 更新逻辑

        isAttacking = false;
    }
    

    List<SoldierInBattle> GetSupportSkillTargets(int targetCount, SupportTargetType supportTargetType,FactionType factionType)
    {
        List<SoldierInBattle> targets = new List<SoldierInBattle>();

        // 如果 技能是指向友方，Target=自己
        // 如果 技能是指向敌方，Target=自己的对立面
        FactionType targetFaction;

        if (factionType == FactionType.Friend)
        {
            // 指向友方 → 选 caster 所在阵营
            targetFaction = this.factionType;
        }
        else
        {
            // 指向敌方 → 反转 caster 阵营
            targetFaction = this.factionType == FactionType.Friend
                ? FactionType.Enemy
                : FactionType.Friend;
        }

        

        switch (supportTargetType)
        {
            case SupportTargetType.Random:
                targets = GetRandomEnemies(targetCount,targetFaction);
                break;
            case SupportTargetType.MaxHp:
                var maxHpTarget = BattleSystem.Instance.battleList
                    .Where(e => !e.isDead && e.factionType == targetFaction)
                    .OrderByDescending(e => e.hp)
                    .FirstOrDefault();

                if (maxHpTarget != null)
                    targets.Add(maxHpTarget);
                break;
            case SupportTargetType.MinHp:
                var minHpTarget = BattleSystem.Instance.battleList
                    .Where(e => !e.isDead && e.factionType == targetFaction)
                    .OrderBy(e => e.hp)
                    .FirstOrDefault();

                if (minHpTarget != null)
                    targets.Add(minHpTarget);
                break;
            case SupportTargetType.MaxPower:
                var maxPowerTarget = BattleSystem.Instance.battleList
                    .Where(e => !e.isDead && e.factionType == targetFaction)
                    .OrderByDescending(e => e.att + e.mag)
                    .FirstOrDefault();

                if (maxPowerTarget != null)
                    targets.Add(maxPowerTarget);
                break;
            case SupportTargetType.Front:
                break;
            case SupportTargetType.Back:
                break;
            case SupportTargetType.All:
                targets = BattleSystem.Instance.battleList
                    .Where(e => !e.isDead && e.factionType == targetFaction)
                    .ToList();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(supportTargetType), supportTargetType, null);
        }
        
        return targets;
    }
    
    List<SoldierInBattle> GetAttackSkillTargets(int targetCount,AttackTargetType attackTargetType)
    {
        List<SoldierInBattle> targets = new();
        switch (attackTargetType)
        {
            case AttackTargetType.Default:
                // 默认目标
                targets.Add(GetDefaultAttackTarget(soldierInstance.formationIndex, factionType)); 
                break;

            case AttackTargetType.Random:
                targets=GetRandomEnemies(targetCount,this.factionType==FactionType.Friend?FactionType.Enemy:FactionType.Friend);
                break;

            case AttackTargetType.Row:
            {
                var defaultTarget = GetDefaultAttackTarget(soldierInstance.formationIndex, factionType);
                if (defaultTarget == null) break;

                int index_Row = defaultTarget.soldierInstance.formationIndex;
                int row = index_Row / 3; // 获取行号

                var enemyList = BattleSystem.Instance.battleList
                    .Where(e => !e.isDead && e.factionType != factionType)
                    .Where(e => e.soldierInstance.formationIndex / 3 == row)
                    .OrderByDescending(e => e.soldierInstance.formationIndex)
                    .ToList();

                targets.AddRange(enemyList);
            }
                break;


            case AttackTargetType.Line:
            {
                var defaultTarget = GetDefaultAttackTarget(soldierInstance.formationIndex, factionType);
                if (defaultTarget == null) break;

                int index = defaultTarget.soldierInstance.formationIndex;
                int col = index % 3;

                var enemyList = BattleSystem.Instance.battleList
                    .Where(e => !e.isDead && e.factionType != factionType)
                    .Where(e => e.soldierInstance.formationIndex % 3 == col)
                    .OrderByDescending(e => e.soldierInstance.formationIndex)
                    .ToList();

                targets.AddRange(enemyList);

                foreach (var t in targets)
                {
                    int i = t.soldierInstance.formationIndex;
                    Debug.Log($"🎯 Line目标: {t.name}, 格子: {i}, 行: {i / 3}, 列: {i % 3}");
                }
            }
                break;

            case AttackTargetType.Front:
            {
                int selfRow = soldierInstance.formationIndex / 3;

                // 定义所有行（从右到左）
                int[][] rows = new int[][]
                {
                    new int[] { 0, 1, 2 }, // 第一排
                    new int[] { 3, 4, 5 }, // 第二排
                    new int[] { 6, 7, 8 }  // 第三排
                };

                // 先处理当前行，放第一个
                var rowOrder = new List<int[]> { rows[selfRow] };

                // 添加其他两行（保持原顺序）
                for (int i = 0; i < rows.Length; i++)
                {
                    if (i != selfRow) rowOrder.Add(rows[i]);
                }

                foreach (var row in rowOrder)
                {
                    foreach (int idx in row)
                    {
                        var unit = BattleSystem.Instance.battleList.FirstOrDefault(e =>
                            !e.isDead &&
                            e.factionType != factionType &&
                            e.soldierInstance.formationIndex == idx);

                        if (unit != null)
                        {
                            targets.Add(unit);
                            break; // 每排只取一个
                        }
                    }
                }
            }
                break;

           case AttackTargetType.Back:
               {
                   int selfRow = soldierInstance.formationIndex / 3;
           
                   // 定义所有行（从左到右）
                   int[][] rows = new int[][]
                   {
                       new int[] { 0, 1, 2 }, // 第一排
                       new int[] { 3, 4, 5 }, // 第二排
                       new int[] { 6, 7, 8 }  // 第三排
                   };
           
                   var rowOrder = new List<int[]> { rows[selfRow] };
                   for (int i = 0; i < rows.Length; i++)
                   {
                       if (i != selfRow) rowOrder.Add(rows[i]);
                   }
           
                   foreach (var row in rowOrder)
                   {
                     
                       foreach (int idx in row)
                       {  
                           var unit = BattleSystem.Instance.battleList.FirstOrDefault(e =>
                               !e.isDead &&
                               e.factionType != factionType &&
                               e.soldierInstance.formationIndex == idx);
           
                           if (unit != null)
                           {
                               targets.Add(unit);
                               break;
                           }
                       }
                   }
               }
               break;

            case AttackTargetType.MaxPower:
            {
                var maxTarget = BattleSystem.Instance.battleList
                    .Where(e => !e.isDead && e.factionType != factionType)
                    .OrderByDescending(e => e.att + e.mag)
                    .FirstOrDefault();

                if (maxTarget != null)
                    targets.Add(maxTarget);
            }
                break;

            case AttackTargetType.MinHp:
            {
                var minTarget = BattleSystem.Instance.battleList
                    .Where(e => !e.isDead && e.factionType != factionType)
                    .OrderBy(e => e.hp)
                    .FirstOrDefault();

                if (minTarget != null)
                    targets.Add(minTarget);
            }
                break;
            case AttackTargetType.MaxHp:
            {
                var maxTarget = BattleSystem.Instance.battleList
                    .Where(e => !e.isDead && e.factionType != factionType)
                    .OrderByDescending(e => e.hp)
                    .FirstOrDefault();

                if (maxTarget != null)
                    targets.Add(maxTarget);
            }
                break;
            case AttackTargetType.All:
            {
                var allEnemies = BattleSystem.Instance.battleList
                    .Where(e => !e.isDead && e.factionType != factionType)
                    .OrderByDescending(e => e.soldierInstance.formationIndex)
                    .ToList();

                targets.AddRange(allEnemies);
            }
                break;

            default:
                Debug.LogWarning("⚠️ 未知的 TargetType！");
                break;
        }
        return targets;
    }
    
    List<SoldierInBattle> GetFrontTarget(SoldierInBattle target, FactionType enemyFaction)
    {
        List<SoldierInBattle> result = new();

        int targetRow = target.soldierInstance.formationIndex / 3;

        // 所有行定义（从右往左）
        int[][] rows = new int[][]
        {
            new int[] { 8, 7, 6 },
            new int[] { 5, 4, 3 },
            new int[] { 2, 1, 0 }
        };

        // 获取目标那一行的敌人
        var sameRowEnemies = BattleSystem.Instance.battleList
            .Where(e => !e.isDead &&
                        e.factionType == enemyFaction &&
                        e.soldierInstance.formationIndex / 3 == targetRow)
            .ToList();

        int[] chosenRow = rows[targetRow];

        // 如果该行没有敌人 → 随机找其他行
        if (sameRowEnemies.Count == 0)
        {
            List<int> otherRows = new List<int> { 0, 1, 2 };
            otherRows.Remove(targetRow);
            int randomRow = otherRows[UnityEngine.Random.Range(0, otherRows.Count)];
            chosenRow = rows[randomRow];
        }

        // 在选定行中，从右往左找第一个敌人
        foreach (int idx in chosenRow)
        {
            var found = BattleSystem.Instance.battleList.FirstOrDefault(e =>
                !e.isDead &&
                e.factionType == enemyFaction &&
                e.soldierInstance.formationIndex == idx);

            if (found != null)
            {
                result.Add(found);
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetCount"></param>
    /// <param name="selfFaction">soldier's faction</param>
    /// <returns></returns>
    List<SoldierInBattle> GetRandomEnemies(int targetCount, FactionType target)
    {
        return BattleSystem.Instance.battleList
            .Where(s => !s.isDead && s.factionType == target)
            .OrderBy(x => Random.value)
            .Take(targetCount)
            .ToList();
    }

    
   
    /// <summary>
    /// 添加 Buff 到角色
    /// </summary>
    public void AddBuff(BuffBaseSO buffSO,SoldierAttributes attributes)
    {
        if (buffSO == null || isDead) return;

        var existing = activeBuffs.FirstOrDefault(b => b.source == buffSO);
        if (existing != null)
            HandleStack(existing, buffSO, attributes);
        else
            CreateNewBuff(buffSO,attributes);

        RefreshBuffGroup();

    }
    
    private void HandleStack(BuffInstance existing, BuffBaseSO buff, SoldierAttributes attributes)
    {
        if (buff.stackAble)
        {
            existing.stackCount=Mathf.Min(existing.stackCount+1, buff.maxStack);
        }

        existing.remainingRounds = buff.duration;
        existing.casterAttributes = attributes;
        
    }

    private void CreateNewBuff(BuffBaseSO buff, SoldierAttributes attributes)
    {
        int newStackCount;
        BuffInstance newBuffInstance;
        
        if(buff.stackAble)
            newBuffInstance = new BuffInstance(buff,this,1,attributes);
        else
            newBuffInstance = new BuffInstance(buff,this,-1,attributes);
        
        activeBuffs.Add(newBuffInstance);
        
    }

    public void RefreshBuffGroup()
    {
        
        foreach(GameObject obj in buffObjects)
            Destroy(obj);

        foreach (var buffData in activeBuffs)
        {
            GameObject newBuffObj = Instantiate(_buffPrefab, buffGroup);
            newBuffObj.GetComponent<Buff>().InitBuff(buffData);
            buffObjects.Add(newBuffObj);
        }
        
    }
    
    /// <summary>
    /// 移除指定的 Buff
    /// </summary>
    public void RemoveBuff(BuffInstance buff)
    {
        if (buff != null && activeBuffs.Contains(buff))
        {
            activeBuffs.Remove(buff);
        }
    }

    /// <summary>
    /// 回合开始时的 Buff 处理（负面 Buff，如中毒等）
    /// </summary>
    public IEnumerator ApplyStartOfTurnDamageBuffs()
    {
        yield return buffSystem.StartOfTurnBuffs();
    }
    
    /// <summary>
    /// 回合结束时的 Buff 处理（正面 Buff，如持续治疗等）
    /// </summary>
    public IEnumerator ApplyEndOfTurnBuffs()
    {
        yield return buffSystem.EndOfTurnBuff();
    }

    /// <summary>
    /// 应用阻止行动
    /// </summary>
    /// <returns></returns>
    public bool ApplyActionBlockBuffs()
    {
        return buffSystem.ActionBlockBuff();
    }

    /// <summary>
    /// 应用魅惑行动
    /// </summary>
    /// <returns></returns>
    public void ApplyActionBetaryBuffs()
    {
        bool isBetrayed = buffSystem.ActionBetrayBuff();
        if(isBetrayed)
            if (factionType == FactionType.Enemy)
                factionType = FactionType.Friend;
            else
                factionType = FactionType.Enemy;
    }

    /// <summary>
    /// 应用限制行为
    /// </summary>
    /// <param name="silenced"></param>
    /// <param name="disarmed"></param>
    public void ApplyActionRestrictBuffs(out bool silenced, out bool disarmed)
    {
        buffSystem.ActionRestriction(out silenced, out disarmed);
    }
    
    /// <summary>
    /// 播放眩晕/受挫动画（低头摇晃）
    /// </summary>
    public IEnumerator PlayStunAnimation()
    {
        // 1. 保存原始状态
        var originPos = transform.localPosition;
        var originRot = transform.localEulerAngles;
        var originScale = transform.localScale;

        // 2. 定义动画序列 (这里放你之前写的 DOTween 代码)
        Sequence stunSeq = DOTween.Sequence();

        // 阶段A: 低头下沉
        stunSeq.Append(transform.DOLocalRotate(new Vector3(0, 0, -30f), 0.2f).SetEase(Ease.OutBack));
        stunSeq.Join(transform.DOScale(new Vector3(originScale.x * 1.1f, originScale.y * 0.9f, 1f), 0.2f));
        stunSeq.Join(transform.DOLocalMoveY(originPos.y - 0.2f, 0.2f));

        // 阶段B: 摇晃循环
        stunSeq.Append(transform.DOLocalRotate(new Vector3(0, 0, -10f), 0.8f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo));
        stunSeq.Join(transform.DOScale(originScale, 0.8f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo));

        // 3. 播放并等待
        stunSeq.Play();
        yield return stunSeq.WaitForCompletion(); // 等待动画播完

        // 4. 还原状态
        transform.localPosition = originPos;
        transform.localEulerAngles = originRot;
        transform.localScale = originScale;
    }
    
    /// <summary>
    /// 播放疑惑/无法行动动画
    /// </summary>
    public IEnumerator PlayConfusedAnimation()
    {
        // 简单做一个左右摇头的动作表示“？？？”
        yield return transform.DOLocalRotate(new Vector3(0, 0, 15f), 0.2f).SetLoops(4, LoopType.Yoyo).WaitForCompletion();
        transform.localEulerAngles = Vector3.zero;
    }

}
