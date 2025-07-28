using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class TracksLine : MonoBehaviour
{
    [Header("Line Settings")]
    public float drawDuration = 1.0f;
    public int curveSegments = 30;
    public float curveHeight = 2f;
    public float lineWidth = 0.2f;
    public Vector3 offset ;
    public Vector3 planeNormal = Vector3.forward;

    [Header("Curve Display Settings")]
    [Tooltip("曲线从固定长度后开始绘制")]
    public float lineStartOffset = 0f;
    [Tooltip("曲线在末尾隐藏的长度")]
    public float lineEndOffset = 0f;

    [Header("LineRenderer Settings")]
    public string sortingLayerName = "Board";
    public int sortingOrder = 1;
    public bool tileMode = true;

    private int lastSign = 0; 


    public void Draw(Vector3 pointA, Vector3 pointB)
    {
        pointA += offset;
        pointB += offset;

        StartCoroutine(DrawCurveCoroutineIndependent(pointA, pointB));
        
    }

    public void LoadLine(List<Vector3> points)
{
    if (points == null || points.Count < 2) return;

    ClearTrack();
    lastSign = 0;

    for (int i = 1; i < points.Count; i++)
    {
        Vector3 pointA = points[i - 1] + offset;
        Vector3 pointB = points[i] + offset;

        if ((pointB - pointA).sqrMagnitude < 1e-6f) continue;

        GameObject lineObj = new GameObject("LineSegment");
        lineObj.transform.parent = transform;
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.textureMode = tileMode ? LineTextureMode.Tile : LineTextureMode.Stretch;
        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = sortingOrder;
        line.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        Material trackMat = Resources.Load<Material>("Materials/Tracks");
        if (trackMat != null)
            line.material = trackMat;
        else
            Debug.LogWarning("Tracks.mat 未找到，请检查路径！");

        // --- 跟 DrawCurveCoroutineIndependent 一样的逻辑 ---
        Vector3 dir = (pointB - pointA).normalized;
        Vector3 segNormal = Vector3.Cross(planeNormal, dir).normalized;

        int sign = (lastSign == 0) ? (Random.value > 0.5f ? 1 : -1) : -lastSign;
        lastSign = sign;

        Vector3 mid = (pointA + pointB) * 0.5f;
        Vector3 control = mid + segNormal * (curveHeight * sign);

        List<Vector3> segmentPoints = new List<Vector3>();
        for (int j = 0; j <= curveSegments; j++)
        {
            float t = j / (float)curveSegments;
            segmentPoints.Add(Bezier(pointA, control, pointB, t));
        }

        // ✅ 应用 lineStartOffset
        float accumulated = 0f;
        int startIndex = 0;
        for (int j = 1; j < segmentPoints.Count; j++)
        {
            accumulated += Vector3.Distance(segmentPoints[j - 1], segmentPoints[j]);
            if (accumulated >= lineStartOffset)
            {
                startIndex = j;
                break;
            }
        }

        // ✅ 应用 lineEndOffset
        accumulated = 0f;
        int endIndex = segmentPoints.Count - 1;
        for (int j = segmentPoints.Count - 1; j > 0; j--)
        {
            accumulated += Vector3.Distance(segmentPoints[j], segmentPoints[j - 1]);
            if (accumulated >= lineEndOffset)
            {
                endIndex = j;
                break;
            }
        }

        // ✅ 截取需要绘制的范围
        if (endIndex > startIndex)
        {
            List<Vector3> finalPoints = segmentPoints.GetRange(startIndex, endIndex - startIndex + 1);
            line.positionCount = finalPoints.Count;
            line.SetPositions(finalPoints.ToArray());
        }
    }
}


    private IEnumerator DrawCurveCoroutineIndependent(Vector3 pointA, Vector3 pointB)
    {
        if ((pointB - pointA).sqrMagnitude < 1e-6f) yield break;

        GameObject lineObj = new GameObject("LineSegment");
        lineObj.transform.parent = transform;
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.textureMode = tileMode ? LineTextureMode.Tile : LineTextureMode.Stretch;
        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = sortingOrder;

        // ✅ 在Mask外可见
        line.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        Material trackMat = Resources.Load<Material>("Materials/Tracks");
        if (trackMat != null)
            line.material = trackMat;
        else
            Debug.LogWarning("Tracks.mat 未找到，请检查路径！");

        Vector3 dir = (pointB - pointA).normalized;
        Vector3 segNormal = Vector3.Cross(planeNormal, dir).normalized;

        int sign = (lastSign == 0) ? (Random.value > 0.5f ? 1 : -1) : -lastSign;
        lastSign = sign;

        Vector3 mid = (pointA + pointB) * 0.5f;
        Vector3 control = mid + segNormal * (curveHeight * sign);

        List<Vector3> segmentPoints = new List<Vector3>();
        for (int i = 0; i <= curveSegments; i++)
        {
            float t = i / (float)curveSegments;
            segmentPoints.Add(Bezier(pointA, control, pointB, t));
        }

        float accumulated = 0f;
        int startIndex = 0;
        for (int i = 1; i < segmentPoints.Count; i++)
        {
            accumulated += Vector3.Distance(segmentPoints[i - 1], segmentPoints[i]);
            if (accumulated >= lineStartOffset)
            {
                startIndex = i;
                break;
            }
        }

        accumulated = 0f;
        int endIndex = segmentPoints.Count - 1;
        for (int i = segmentPoints.Count - 1; i > 0; i--)
        {
            accumulated += Vector3.Distance(segmentPoints[i], segmentPoints[i - 1]);
            if (accumulated >= lineEndOffset)
            {
                endIndex = i;
                break;
            }
        }

        float wait = drawDuration / Mathf.Max(1, endIndex - startIndex + 1);
        List<Vector3> pointsToDraw = new List<Vector3>();
        for (int i = startIndex; i <= endIndex; i++)
        {
            pointsToDraw.Add(segmentPoints[i]);
            line.positionCount = pointsToDraw.Count;
            line.SetPositions(pointsToDraw.ToArray());
            yield return new WaitForSeconds(wait);
        }
    }

    private Vector3 Bezier(Vector3 a, Vector3 c, Vector3 b, float t)
    {
        Vector3 ab = Vector3.Lerp(a, c, t);
        Vector3 cb = Vector3.Lerp(c, b, t);
        return Vector3.Lerp(ab, cb, t);
    }

    public void ClearTrack()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        lastSign = 0;
    }
}
