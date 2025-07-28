using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;[RequireComponent(typeof(CanvasGroup))]
public class SoldierInBattleSettings : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SoldierInstance soldier;
    public Image soldierImage;

    [HideInInspector] public Transform parentAfterDarg;

    private Vector3 originalPosition;
    private Transform originalParent;
    private Transform gridContainer;               // 九宫格容器
    private Transform soldierSettingGroup;         // 左侧备战区容器

    // 新增：缓存自身的 RectTransform 和 Canvas，用于正确的坐标转换
    private RectTransform _rectTransform;
    private Canvas _canvas;

    private SoldierBattleSettingsPanelAdapter _soldierBattleSettingsPanelAdapter;
    private void Start()
    {
        gridContainer = GameObject.Find("NineGridContainer").transform;
        soldierSettingGroup = GameObject.Find("SoldierSettingGroup").transform;

        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void Init(SoldierInstance soldierInstance,SoldierBattleSettingsPanelAdapter adapter)
    {
        _soldierBattleSettingsPanelAdapter = adapter;
        soldier = soldierInstance;
        soldierImage.sprite = soldier.soldierData.soldier_Sprite;
        
        soldierImage.preserveAspect = true;
        soldierImage.SetNativeSize();
        soldierImage.transform.localScale = Vector3.one * 0.05f;  
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        soldierImage.raycastTarget = false;
        originalParent = transform.parent;
        originalPosition = transform.position;

        // 【修复点1】：绝对不能用 transform.root！必须放到 Canvas 节点下才能正常渲染
        if (_canvas != null)
        {
            transform.SetParent(_canvas.transform, true);
        }
        transform.SetAsLastSibling(); // 提到最顶层，防止被遮挡
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 【修复点2】：Camera 模式下，必须把屏幕坐标转换为画板上的世界坐标！
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _canvas.transform as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector3 worldPoint))
        {
            _rectTransform.position = worldPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        soldierImage.raycastTarget = true;

        // 【修复点3】：传入事件自带的相机，否则 Camera 模式下距离计算全是错的！
        Transform targetSlot = GetNearestSlotWithinDistance(100f, eventData.pressEventCamera, eventData.position);

        if (targetSlot != null)
        {
            int targetIndex = ParseSlotIndex(targetSlot.name);
            if (targetIndex == -1)
            {
                Debug.LogWarning("无效的槽位名称，无法解析formationIndex");
                ResetPosition();
                return;
            }

            Transform existingSoldier = null;
            if (targetSlot.childCount > 0)
            {
                existingSoldier = targetSlot.GetChild(0);
            }

            if (existingSoldier != null)
            {
                SwapSoldiers(existingSoldier, this.transform, targetIndex);
                _soldierBattleSettingsPanelAdapter.RefreshFormation();
            }
            else
            {
                SetSoldierToSlot(this.transform, targetSlot, targetIndex);
                _soldierBattleSettingsPanelAdapter.RefreshFormation();
            }
        }
        else
        {
            // 判断是否拖回左侧备战区
            if (IsPointerOverTransform(eventData.position, soldierSettingGroup, 150f, eventData.pressEventCamera))
            {
                ReturnToSoldierSettingGroup();
                _soldierBattleSettingsPanelAdapter.RefreshFormation();
            }
            else
            {
                ResetPosition();
            }
        }
    }

    private void SetSoldierToSlot(Transform soldierTransform, Transform slot, int formationIndex)
    {
        soldierTransform.SetParent(slot);
        soldierTransform.localPosition = Vector3.zero;

        SoldierInBattleSettings soldierSetting = soldierTransform.GetComponent<SoldierInBattleSettings>();
        if (soldierSetting != null && soldierSetting.soldier != null)
        {
            soldierSetting.soldier.formationIndex = formationIndex;
            soldierSetting.RefreshPosition();
        }
    }

    private void SwapSoldiers(Transform existingSoldier, Transform draggedSoldier, int targetIndex)
    {
        var draggedSetting = draggedSoldier.GetComponent<SoldierInBattleSettings>();
        var existingSetting = existingSoldier.GetComponent<SoldierInBattleSettings>();

        if (draggedSetting == null || existingSetting == null)
        {
            Debug.LogWarning("角色组件缺失，无法互换");
            ResetPosition();
            return;
        }

        Transform draggedOldParent = draggedSetting.originalParent;
        Transform existingOldParent = existingSetting.originalParent;

        existingSoldier.SetParent(draggedOldParent);
        existingSoldier.localPosition = Vector3.zero;
        existingSetting.soldier.formationIndex = GetFormationIndexByParent(draggedOldParent);

        draggedSoldier.SetParent(gridContainer.Find($"NineGridSlot_{targetIndex + 1}"));
        draggedSoldier.localPosition = Vector3.zero;
        draggedSetting.soldier.formationIndex = targetIndex;

        draggedSetting.RefreshPosition();
        existingSetting.RefreshPosition();
    }

    private int GetFormationIndexByParent(Transform parent)
    {
        if (parent == null) return -1;

        if (parent.parent == gridContainer)
        {
            return ParseSlotIndex(parent.name);
        }
        else if (parent == soldierSettingGroup)
        {
            return -1;
        }
        else
        {
            return -1;
        }
    }

    private void ReturnToSoldierSettingGroup()
    {
        transform.SetParent(soldierSettingGroup);
        transform.localPosition = Vector3.zero;

        if (soldier != null)
        {
            soldier.formationIndex = -1;
        }
        RefreshPosition();
    }

    private void ResetPosition()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
    }

    private int ParseSlotIndex(string slotName)
    {
        if (!slotName.StartsWith("NineGridSlot_")) return -1;

        string numStr = slotName.Substring("NineGridSlot_".Length);
        if (int.TryParse(numStr, out int idx))
        {
            return idx - 1; 
        }
        return -1;
    }

    /// <summary>
    /// 【修复点4】：新增 camera 和 pointerPos 参数，完美适配 Camera 模式
    /// </summary>
    private Transform GetNearestSlotWithinDistance(float maxDistance, Camera cam, Vector2 pointerPos)
    {
        float minDistance = float.MaxValue;
        Transform nearestSlot = null;

        for (int i = 1; i <= 9; i++)
        {
            string slotName = $"NineGridSlot_{i}";
            Transform slot = gridContainer.Find(slotName);

            if (slot == null) continue;

            // Camera 模式下，必须传入真实的 Camera，不能传 null！
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, slot.position);
            float distance = Vector2.Distance(pointerPos, screenPos);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSlot = slot;
            }
        }

        if (minDistance <= maxDistance)
            return nearestSlot;

        return null;
    }

    /// <summary>
    /// 【修复点5】：新增 camera 参数，解决备战区距离检测失效的问题
    /// </summary>
    private bool IsPointerOverTransform(Vector2 pointerPos, Transform targetTransform, float radius, Camera cam)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, targetTransform.position);
        return Vector2.Distance(pointerPos, screenPos) <= radius;
    }

    public void RefreshPosition()
    {
        if (soldier == null) return;

        if (soldier.formationIndex >= 0 && soldier.formationIndex <= 8)
        {
            Transform slot = gridContainer.Find($"NineGridSlot_{soldier.formationIndex + 1}");
            if (slot != null)
            {
                transform.SetParent(slot);
                transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogWarning($"找不到对应的九宫格槽位: NineGridSlot_{soldier.formationIndex + 1}");
            }
        }
        else
        {
            transform.SetParent(soldierSettingGroup);
            transform.localPosition = Vector3.zero;
        }
    }
}