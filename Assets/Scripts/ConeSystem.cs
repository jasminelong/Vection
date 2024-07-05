using UnityEngine;

public class ConeGenerator : MonoBehaviour
{
    public Camera User_Camera;
    public GameObject dotPrefab; // 白色小圆点的预制体
    public GameObject vertexPrefab; // 红色顶点的预制体

    public float newDotRadius = 0.005f; // 将小圆点半径设置为新的值，此时半径为0.5cm 

    public float coneHeight = 20f; // 圆锥的高度，m
    public float coneRadius = 1f; // 圆锥的底部半径，m
    public float cameraSpeed = 0.01f; // 摄像机沿圆锥轴线移动的速度，m/s
    public float totalDotAreaPerSurfaceArea = 0.04f; // 圆锥表面积内所有小圆点的总面积比例

    private GameObject vertexDot; // 顶点的引用
    private Transform cameraTransform; // 主摄像机的Transform
    private Vector3 coneBaseCenter; // 圆锥底部的中心位置
    private Vector3 coneVertexPosition; // 圆锥顶点的中心位置

    void Start()
    {
        Vector3 coneBaseCenter = Vector3.zero; // 圆锥底部的中心位置设为原点
        coneVertexPosition = new Vector3(0f, 0f, coneHeight); // 圆锥顶点位置设为z轴远处
        User_Camera.transform.position = coneBaseCenter; // 相机初始位置设为原点
        cameraTransform = User_Camera.transform; // 获取主摄像机的Transform

        GenerateDots();
        PlaceVertex();
    }

    void Update()
    {
        MoveCameraAlongConeAxis();
    }

    void GenerateDots()
    {
        // 计算圆锥的表面积
        float surfaceArea = Mathf.PI * coneRadius * Mathf.Sqrt(coneRadius * coneRadius + coneHeight * coneHeight);

        // 计算所有小圆点的总面积
        float totalDotArea = surfaceArea * totalDotAreaPerSurfaceArea;

        // 计算每个小圆点的实际面积
        float dotArea = Mathf.PI * newDotRadius * newDotRadius;

        //计算小圆点的数量
        int numDots = Mathf.RoundToInt(totalDotArea / dotArea);

        //设置小圆点的半径
        SphereCollider collider = dotPrefab.GetComponent<SphereCollider>();

        if (collider != null)
        {
            // initialRadius 是预制体的初始半径
            float initialRadius = collider.radius;

            // 获取小圆点预制体的 Transform 组件
            Transform dotTransform = dotPrefab.transform;

            // 计算新的缩放比例
            float scaleFactor = newDotRadius / initialRadius;

            // 设置小圆点预制体的缩放
            dotTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
        
        for (int i = 0; i < numDots; i++)
        {
            // 在圆锥表面上随机生成一个点
            float theta = Random.Range(0f, Mathf.PI * 2f); // 随机角度
            float z = Random.Range(0f, coneHeight); // 随机高度
            float radius = (coneHeight - z ) / coneHeight * coneRadius; // 当前高度的半径

            // 计算圆柱坐标系中的位置
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);

            // 在计算出的位置上实例化白色小圆点预制体
            Vector3 position = new Vector3(x, y, z);
            GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity);
            dot.transform.SetParent(transform); // 将其作为圆锥的父对象
        }
    }

    void PlaceVertex()
    {
        // 在圆锥顶部实例化红色顶点预制体
        /*vertexDot = Instantiate(vertexPrefab, coneVertexPosition, Quaternion.identity);
        vertexDot.transform.SetParent(transform); // 将其作为圆锥的子对象*/
    }

    void MoveCameraAlongConeAxis()
    {
        // 计算摄像机沿圆锥轴线移动的目标位置
        float distance = Vector3.Distance(cameraTransform.position, coneVertexPosition);
        Vector3 direction = (coneVertexPosition - cameraTransform.position).normalized;
        Vector3 targetPosition = cameraTransform.position + direction * cameraSpeed * Time.deltaTime * distance;

        // 移动摄像机到目标位置
        //cameraTransform.position = targetPosition;
        // 移动摄像机到目标位置
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, coneVertexPosition, Time.deltaTime * cameraSpeed);

        // 确保摄像机始终朝向圆锥顶点
        cameraTransform.LookAt(coneVertexPosition);

        // 确保远处显示红色顶点
        if (vertexDot != null)
        {
            vertexDot.transform.position = coneVertexPosition;
        }
    }
}
