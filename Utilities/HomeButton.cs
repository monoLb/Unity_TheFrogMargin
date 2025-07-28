using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class HomeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Image buttonImage;

    public Sprite homeButtonSprite_true;
    public Sprite homeButtonSprite_false;

    [Header("Panel")] public GameObject panel;

    public Color originColor;
    public Color EnterColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = EnterColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.sprite = homeButtonSprite_false;
        buttonImage.color = originColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
        StartCoroutine(HandleClick());
    }

    private IEnumerator HandleClick()
    {
        // 按下时更换图片
        buttonImage.sprite = homeButtonSprite_true;

        // 等待0.5秒
        yield return new WaitForSeconds(0.2f);

        // 恢复原始图片与颜色
        buttonImage.sprite = homeButtonSprite_false;
        buttonImage.color = originColor;
        

    }
}