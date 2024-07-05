using UnityEngine;

public class CylinderGenerator : MonoBehaviour
{
    public GameObject dotPrefab; // 白色小圆点的预制体

    public float newDotRadius = 0.005f; // 将小圆点半径设置为新的值，此时半径为0.5cm 

    public float cylinderHeight = 20f; // 圆柱的高度，m
    public float cylinderRadius = 1f; // 圆柱的底部半径，m

    public float totalDotAreaRatio = 0.04f; // 圆柱侧面积内所有小圆点的总面积比例


    public Vector3 cylinderBaseCenter = Vector3.zero; // 圆柱底部的中心位置设为原点
    public Vector3 cylinderTopCenter; // 圆柱顶部的中心位置


    void Start()
    {
        cylinderTopCenter = new Vector3(0f, 0f, cylinderHeight); // 圆柱顶部位置设为高度的顶点

        GenerateDots();
    }

    void Update()
    {
        

    }

    void GenerateDots()
    {
        // 计算圆柱的侧面积
        float surfaceArea = 2 * Mathf.PI * cylinderRadius * cylinderHeight;

        // 计算所有小圆点的总面积
        float totalDotArea = surfaceArea * totalDotAreaRatio;

        // 计算每个小圆点的实际面积
        float dotArea = Mathf.PI * newDotRadius * newDotRadius;

        // 计算小圆点的数量
        int numDots = Mathf.RoundToInt(totalDotArea / dotArea);

        // 设置小圆点的半径
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
        // 创建红色顶点
         Color newColor = Color.red;
        GameObject redVertex = Instantiate(dotPrefab, cylinderTopCenter, Quaternion.identity);
        redVertex.GetComponent<Renderer>().material.color = newColor;
        redVertex.transform.SetParent(transform);

        //随机生成小白点
        for (int i = 0; i < numDots; i++)
        {
            // 在圆柱侧面上随机生成一个点
            float theta = Random.Range(0f, Mathf.PI * 2f); // 随机角度
            float height = Random.Range(0f, cylinderHeight); // 随机高度

            // 计算圆柱坐标系中的位置
            float x = cylinderRadius * Mathf.Cos(theta);
            float y = cylinderRadius * Mathf.Sin(theta);

            // 在计算出的位置上实例化白色小圆点预制体
            Vector3 position = new Vector3(x, y, height);
            GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity);
            dot.transform.SetParent(transform); // 将其作为圆柱的父对象
        }
    }
   
}
