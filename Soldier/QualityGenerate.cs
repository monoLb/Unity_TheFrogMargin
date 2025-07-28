using System.Collections.Generic;
using UnityEngine;

public struct QualityRate
{
    public int green;
    public int blue;
    public int purple;
    public int red;
    public int gold;

    public QualityRate(int g, int b, int p, int r, int o)
    {
        green = g; blue = b; purple = p; red = r; gold = o;
    }
}

public class QualityGenerate 
{
    private List<QualityRate> qualityTable = new List<QualityRate>()
    {
        new QualityRate(99, 1, 0, 0, 0),     // Chapter 0
        new QualityRate(95, 5, 0, 0, 0),     // Chapter 1
        new QualityRate(85, 10, 0, 0, 0),    // Chapter 2
        new QualityRate(80, 15, 5, 0, 0),    // Chapter 3
        new QualityRate(75, 20, 10, 0, 0),   // Chapter 4
        new QualityRate(60, 25, 15, 0, 0),   // Chapter 5
        new QualityRate(45, 30, 20, 5, 0),   // Chapter 6
        new QualityRate(30, 35, 25, 10, 0),  // Chapter 7
        new QualityRate(25, 30, 30, 15, 0),  // Chapter 8
        new QualityRate(15, 25, 25, 25, 5),  // Chapter 9
        new QualityRate(15, 20, 25, 30, 10), // Chapter 10
    };

    public QualityType GetQuality(int chapter)
    {
        int index = Mathf.Clamp(chapter, 0, qualityTable.Count - 1);
        var rate = qualityTable[index];
        return WeightedRandom(rate.green, rate.blue, rate.purple, rate.red, rate.gold);
    }

    private QualityType WeightedRandom(int green, int blue, int purple, int red, int gold)
    {
        int total = green + blue + purple + red + gold;
        int roll = Random.Range(0, total);
        int cumulative = 0;

        if ((cumulative += green) > roll) return QualityType.Green;
        if ((cumulative += blue) > roll) return QualityType.Blue;
        if ((cumulative += purple) > roll) return QualityType.Purple;
        if ((cumulative += red) > roll) return QualityType.Red;
        return QualityType.Gold;
    }
}
        
    
