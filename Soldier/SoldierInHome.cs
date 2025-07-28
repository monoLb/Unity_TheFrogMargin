using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SoldierInHome : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public SoldierInstance soldier;
    
    [Header("Set")]
    public QualityDataSO qualityDataSO;
    public Image soldierImage;
    public GameObject selectedIcon;
    public GameObject battleIcon;
    
    [Header("Get")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI magicText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI elementText;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDescription;

 
    public Image qualityImage;
    public bool isSelected = false;
    
    private SoldierHomePanelAdapter _soldierHomePanelAdapter;
    private void Start()
    {
        nameText=GameObject.Find("Text_Home_Name").GetComponent<TextMeshProUGUI>();
        healthText=GameObject.Find("Text_Home_HP").GetComponent<TextMeshProUGUI>();
        attackText=GameObject.Find("Text_Home_Att").GetComponent<TextMeshProUGUI>();
        magicText=GameObject.Find("Text_Home_Mag").GetComponent<TextMeshProUGUI>();
        speedText=GameObject.Find("Text_Home_Spe").GetComponent<TextMeshProUGUI>();
        elementText=GameObject.Find("Text_Home_Element").GetComponent<TextMeshProUGUI>();
        skillNameText=GameObject.Find("Text_Home_Skill_Name").GetComponent<TextMeshProUGUI>();
        skillDescription=GameObject.Find("Text_Home_Skill_Description").GetComponent<TextMeshProUGUI>();
        
        qualityImage=GameObject.Find("Image_Home_Quality").GetComponent<Image>();
    }
    
    public void Init(SoldierInstance soldierInstance,SoldierHomePanelAdapter adapter)
    {
        _soldierHomePanelAdapter = adapter;
        soldier = soldierInstance;
        soldierImage.sprite = soldier.soldierData.soldier_Sprite;
        
        soldierImage.preserveAspect = true;
        soldierImage.SetNativeSize();
        soldierImage.transform.localScale = Vector3.one * 0.09f;  
        
        if(soldier.isBattle)
            battleIcon.SetActive(true);
        else
            battleIcon.SetActive(false);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        nameText.text = soldier.soldierData.soldier_Name;
        healthText.text = soldier.health.ToString();
        attackText.text = soldier.attack.ToString();
        magicText.text = soldier.magic.ToString();
        speedText.text = soldier.speed.ToString();
        
        elementText.fontSize = soldier.elementList.Count switch
        {
            >= 5 => 22,
            >= 3 => 25,
            _    => 30
        };
        elementText.text = string.Join(" ", soldier.elementList.Select(e => e.ToString()));
        
        skillNameText.text = soldier.soldierData.soldier_SkillName;
        skillDescription.text = soldier.soldierData.soldier_SkillDescription;

        qualityImage.sprite = qualityDataSO.GetQualitySprite(soldier.soldierData.soldier_Quantity);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!soldier.isBattle)
        {
            isSelected=!isSelected;
            if (isSelected&&PlayerSaveData.Instance.globalData.maxBattleSoldierCount
                >PlayerSaveData.Instance.globalData.battleList.Count+_soldierHomePanelAdapter.selectedSoldierList.Count)
            {
                selectedIcon.SetActive(true);
                _soldierHomePanelAdapter.selectedSoldierList.Add(this);
            }
            else
            {
                selectedIcon.SetActive(false);
                _soldierHomePanelAdapter.selectedSoldierList.Remove(this);
            }
        }
        else
        {
            isSelected=!isSelected;
            if(isSelected)
            {
                selectedIcon.SetActive(true);
                _soldierHomePanelAdapter.selectedSoldierList.Add(this);
            }
            else
            {
                selectedIcon.SetActive(false);
                _soldierHomePanelAdapter.selectedSoldierList.Remove(this);
            }
        }
        
    }

    
}
