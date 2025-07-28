using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LootSlot : MonoBehaviour
{
    [SerializeField] private Image lootBackground;
    [SerializeField] private Image lootImage;
    [SerializeField] private TextMeshProUGUI lootCount;
    private ResultBagItem _lootItem;

    public void Init(ResultBagItem item)
    {
        _lootItem = item;
        
        if(_lootItem == null)
            return;

        switch (_lootItem.bagType)
        {
            case ResultType.Currency:
                lootBackground.sprite=Resources.Load<Sprite>("Sprites/UI/Loot_Currency");
                break;
            case ResultType.Soldier:
                lootBackground.sprite=Resources.Load<Sprite>("Sprites/UI/Loot_Soldier");
                break;
            case ResultType.QuestScroll:
                lootBackground.sprite=Resources.Load<Sprite>("Sprites/UI/Loot_QuestScroll");
                break;
            case ResultType.StoryFragment:
                lootBackground.sprite=Resources.Load<Sprite>("Sprites/UI/Loot_StoryFragment");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        lootImage.sprite = _lootItem.batItem.Result_Sprite;
        //TODO 总结图片的显示方案
        lootImage.preserveAspect = true;
        lootImage.SetNativeSize();
        lootImage.transform.localScale = Vector3.one * 0.05f; 
        
        lootCount.text=_lootItem.bagCount.ToString();

    }
    
}
