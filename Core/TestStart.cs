using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestStart : MonoBehaviour
{
    public static TestStart Instance;
    public ResultSystem resultSystem;
    public List<ResultBagItem> bag = new();


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        bag = resultSystem.GetResultBag(1, 1);
        
        UIFrameRoot.GetInstance().UIManager_Root.Pop(true);

        UIManager.GetInstance().CanvasObj = UIMethods.GetInstance().FindCanvas();
        UIManager.GetInstance().Push(new Result_Panel(true));
    }
}
