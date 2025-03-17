using UnityEngine;
using UnityEngine.UIElements;

public class HaloLine : MonoBehaviour
{ 
    public int segmentCount = 36;  // 椭圆的分段数（点的数量）
    public float xRadius = 3f;     // 椭圆的 X 轴半径
    public float yRadius = 2f;     // 椭圆的 Y 轴半径
    public float moveSpeed = 2f;   // 运动速度（控制虚线滚动）
    public Transform _CenterPos;
    public LineRenderer lineRenderer;
    public float offset = 0f;
    
    public bool isSkill;
    public float srollSpeed = 2f;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.transform.position = _CenterPos.position;
        lineRenderer.positionCount = segmentCount;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        // lineRenderer.startColor = Color.red;
        // lineRenderer.endColor = Color.red;

        
        DrawDottedEllipse();
    }

    void Update()
    {
        if (!isSkill)
        {
            lineRenderer.material.color=Color.white;
            offset += moveSpeed * Time.deltaTime;
        }
        // 让虚线沿着椭圆滚动
        if (isSkill)
        {
            
            lineRenderer.material.color=new Color(244/255f,65/255f,65/255f);
            offset += -moveSpeed * Time.deltaTime*srollSpeed;
        }
        DrawDottedEllipse();
    }

    void DrawDottedEllipse()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            float angle = (i + offset) * 2 * Mathf.PI / segmentCount; // 角度偏移，制造运动感
            float x = Mathf.Cos(angle) * xRadius+_CenterPos.position.x;
            float y = Mathf.Sin(angle) * yRadius+_CenterPos.position.y;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
    
    
}
