using System;
using TMPro;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    static public StateManager Instance;
    public TextMeshProUGUI MyCoin;

    private void Awake()
    {
  
        Instance = this;
   
    }

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        MyCoin.text = GameManager.Instance.Coin.ToString();
        if(GameManager.Instance.Coin<=0)
            MyCoin.text = "没米还来沾边？";
    }
}
