using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ResultBagItem
{
    public ResultItem batItem;
    public int bagCount;
    public ResultType bagType;

    public ResultBagItem(ResultItem batItem, int bagCount, ResultType bagType)
    {
        this.batItem = batItem;
        this.bagCount = bagCount;
        this.bagType = bagType;
    }
}
public class ResultSystem : MonoBehaviour
{
    public List<ResultBagItem> bag =new ();
    
    public ResultDataSO resultData; // 从Inspector挂入SO

    /// <summary>
    /// 获取固定收益
    /// </summary>
    /// <param name="chapterNumber">章节号</param>
    /// <param name="floorNumber">关卡号</param>
    /// <returns>收益数值</returns>
    public int GetFixedResult(int chapterNumber, int floorNumber)
    {
        int fixedResult = 0;
        int stageCount = chapterNumber / 3;

        if (floorNumber != 3)
        {
            fixedResult = 150 + (chapterNumber - 1) * 100 + stageCount * 150 - stageCount * 100;
        }
        else
        {
            fixedResult = 300 + (chapterNumber - 1) * 100 + stageCount * 150 - stageCount * 100;
        }

        Debug.Log($"c {chapterNumber}+f {floorNumber}+RESULT => {fixedResult}");
        return fixedResult;
    }
    

    /// <summary>
    /// 根据目标金额随机生成物品列表，总金额在 ±20% 内
    /// </summary>
    /// <param name="targetAmount">目标金额</param>
    /// <returns>生成的物品列表</returns>
    public List<ResultBagItem> GenerateRandomItems(int targetAmount)
    {
        bag.Clear(); // 清空旧结果

        int minTotal = Mathf.RoundToInt(targetAmount * 0.8f);
        int maxTotal = Mathf.RoundToInt(targetAmount * 1.2f);

        int total = 0;
        System.Random rand = new System.Random();
        int safeCount = 1000; // 安全次数，避免死循环

        while (total < minTotal && safeCount-- > 0)
        {
            // 随机挑选一个物品
            ResultItem randomItem = resultData.currencies[rand.Next(resultData.currencies.Count)];

            // 如果加上后超过 maxTotal，跳过
            if (total + randomItem.Result_Value > maxTotal)
                continue;

            // 查找是否已存在
            ResultBagItem existing = bag.Find(x => x.batItem == randomItem);
            if (existing != null)
            {
                existing.bagCount++;
            }
            else
            {
                bag.Add(new ResultBagItem(randomItem, 1,ResultType.Currency));
            }

            total += randomItem.Result_Value;
        }

        Debug.Log($"Generated items total value: {total} (target={targetAmount}, range={minTotal}-{maxTotal})");
        return bag;
    }
    

    public List<ResultBagItem> GetResultBag(int chapterNumber, int floorNumber)
    {
        int money= GetFixedResult(chapterNumber, floorNumber);
        GenerateRandomItems(money);
        List<ResultBagItem> bag = GenerateRandomItems(money);
        foreach (var item in bag)
        {
            Debug.Log(item.batItem.Result_Name);
            Debug.Log(item.bagCount);
        }
        return bag;
        
    }
    
   
}
