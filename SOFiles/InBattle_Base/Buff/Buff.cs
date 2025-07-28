using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Buff : MonoBehaviour
{
    public Image _buffImage;
    public TextMeshProUGUI _stackCount;

    private void Awake()
    {
        _buffImage=transform.GetChild(0).GetComponent<Image>();
        _stackCount=transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void InitBuff(BuffInstance buffData)
    {
        _buffImage.sprite = buffData.source.buffIcon;
        if (buffData.source.stackAble)
            _stackCount.text = buffData.stackCount.ToString();
        else
            _stackCount.text = "";
    }

    public void UpdateBuff(int stackCount)
    {
        _stackCount.text = stackCount.ToString();
    }
}
