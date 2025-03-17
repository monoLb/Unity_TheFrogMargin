using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class EnemyIcon : MonoBehaviour
{
    public SpriteRenderer background;//闪烁
    public GameObject confirmPanel;//确认战斗
    [Header("属性")] 
    public float rotateAngle = 720f; // 旋转角度
    public float scaleSize = 2f; // 目标放大倍数
    public float duration = 2f;
    public float jumpPower = 2f; // 跳跃的高度
    public float jumpDuration = 1f; // 每次跳跃的持续时间
    public float moveDistance = 3f; // 每次跳跃的水平距离

    void Start()
    {
        // 创建循环动画
        CreateLoopingJumpAnimation();
    }

    void CreateLoopingJumpAnimation()
    {
        // 创建一个动画序列
        Sequence jumpSequence = DOTween.Sequence();

        // 往左跳跃
        jumpSequence.Append(transform.DOMoveY(transform.position.y + jumpPower, jumpDuration / 2)
                .SetEase(Ease.OutQuad))  // 向上跳跃
            .Append(transform.DOMoveY(transform.position.y, jumpDuration / 2)
                .SetEase(Ease.InQuad))  // 下落回到原位置
            .Append(transform.DOMoveX(transform.position.x, jumpDuration) // 移动回原位置
                .SetEase(Ease.InOutSine));
        

        // 设置这个序列永远循环
        jumpSequence.SetLoops(-1, LoopType.Restart);
    }

    private void OnMouseDown()
    {
        OnPointerClick();
    }

    public void OnPointerClick()
    {
        confirmPanel.SetActive(true);
        confirmPanel.transform.DORotate(new Vector3(0, 0, rotateAngle), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutCubic); // 使用缓动函数使动画更流畅

        // 在 scaleDuration 时间内平稳放大到目标大小
        confirmPanel.transform.DOScale(scaleSize, duration).SetEase(Ease.OutBack);

    }
}
