using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class NineGridItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SoldierInstance soldier;

    [Header("Set")]
    public Image soldierImage;


    [HideInInspector] public Transform parentAfterDarg;
    public void Init(SoldierInstance soldierInstance)
    {
        soldier = soldierInstance;
        soldierImage.sprite = soldier.soldierData.soldier_Sprite;
    }
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        soldierImage.raycastTarget = false;
        parentAfterDarg=transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position= Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        soldierImage.raycastTarget = true;
        transform.SetParent(parentAfterDarg);
    }
}
