using UnityEngine;

[CreateAssetMenu(fileName = "QualityDataSO", menuName = "Core/QualityDataSO")]
public class QualityDataSO : ScriptableObject
{
    public Sprite quality_Green;
    public Sprite quality_Blue;
    public Sprite quality_Purple;
    public Sprite quality_Red;
    public Sprite quality_Gold;
    public Sprite quality_Default;

    public Sprite GetQualitySprite(QualityType quality)
    {
        switch (quality)
        {
            case QualityType.Green:
                return quality_Green;
            case QualityType.Blue:
                return quality_Blue;
            case QualityType.Purple:
                return quality_Purple;
            case QualityType.Red:
                return quality_Red;
            case QualityType.Gold:
                return quality_Gold;
            default:
                return quality_Default;
        }
    }
}