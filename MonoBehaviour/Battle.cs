using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;


public class Battle : MonoBehaviour
{
    public Vector3 originPos;
    public Vector3 center;

    public float xRadius;
    public float yRadius;
    public float jumpForce=2f;
    public float jumpDuration=0.5f;
    public float moveScale = 0.3f; // 限制最大移动范围（0.0~1.0），数值越小，移动范围越小
    public Faction faction;
    public float offset = 0.5f;
    public bool isAttack;
    
    public Sequence jumpSequence;
    void Start()
    {
        xRadius = BattleManager.Instance.xRadius;
        yRadius = BattleManager.Instance.yRadius;
        
        if (faction == Faction.friend)
            center = BattleManager.Instance.PlayerPos.position;
        else if(faction == Faction.enemy)
            center = BattleManager.Instance.EnemyPos.position;
        JumpToRandomPoint();
    }
    void JumpAndJump(Vector3 originPos)
    {
        jumpSequence = DOTween.Sequence();
        jumpSequence.Append(transform.DOMoveY(transform.position.y + jumpForce, jumpDuration / 2)
                .SetEase(Ease.OutQuad))
            .Append(transform.DOMoveY(originPos.y, jumpDuration / 2)
                .SetEase(Ease.InQuad))
            .Join(transform.DOMoveX(originPos.x, jumpDuration)
                .SetEase(Ease.InOutSine))
            .OnComplete(() => JumpToRandomPoint());  // 跳跃完成后再次跳跃
    }
    void JumpToRandomPoint()
    {
        if(isAttack)
            return;
        Vector3 currentPos = transform.position;
    
        // 计算当前位置距离椭圆中心的距离
        float distanceToCenter = Vector3.Distance(currentPos, center);
        // 判断是否接近椭圆边缘
        bool isNearEdge = distanceToCenter > (xRadius + yRadius) / 2f - offset;
        // 如果接近边缘，指向中心的方向
        float angle = isNearEdge ? Mathf.Atan2(center.y - currentPos.y, center.x - currentPos.x) : Random.Range(0f, 2f * Mathf.PI);
        // 计算跳跃目标位置
        float moveX = moveScale * Mathf.Cos(angle);
        float moveY = moveScale * Mathf.Sin(angle);
        Vector3 targetPosition = currentPos + new Vector3(moveX, moveY);
        // 确保目标位置在椭圆范围内
        float clampedX = Mathf.Clamp(targetPosition.x, center.x - xRadius, center.x + xRadius);
        float clampedY = Mathf.Clamp(targetPosition.y, center.y - yRadius, center.y + yRadius);
        targetPosition = new Vector3(clampedX, clampedY);
        
        JumpAndJump(targetPosition);
    }
    
    public void AttackTheTarget(Transform target)
    {
        originPos = this.transform.position;
        // 设置攻击旋转
        this.transform.DOLocalRotate(new Vector3(0f, 0f, -63f), 1f);
        
        // 攻击目标位置
        this.transform.DOMove(target.position, 0.5f).SetEase(Ease.Flash).OnKill(() =>
        {
            this.transform.DOMove(originPos, 1f).SetEase(Ease.Unset);
            this.transform.DOLocalRotate(Vector3.zero, 0.5f);
        }).OnComplete(()=>
        {
            BattleManager.Instance.RefreshPreparationUI();
            BattleManager.Instance.RefreshBloodUI();
            isAttack = false;
            JumpAndJump(originPos);
        });
        
    }
    
}
