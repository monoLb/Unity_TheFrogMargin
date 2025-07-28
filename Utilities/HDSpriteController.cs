using UnityEngine;

public class HDSpriteController : MonoBehaviour
{
    private SpriteRenderer sr;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void ApplyHDSprite(Sprite sprite)
    {
        if (sprite == null || sr == null) return;

        sr.GetPropertyBlock(mpb);

        // 设置贴图（也可以设置颜色等属性）
        mpb.SetTexture("_MainTex", sprite.texture); // 视你的 shader 属性名而定
        sr.SetPropertyBlock(mpb);
    }
}