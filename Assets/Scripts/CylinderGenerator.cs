using UnityEngine;
using System.Collections.Generic;

public class CylinderGenerator : MonoBehaviour
{
    public GameObject dotPrefab; // 白色小圆点的预制体

    public float newDotRadius = 0.01f; // 将小圆点半径设置为新的值，此时半径为0.5cm 

    public float cylinderHeight = 20f; // 圆柱的高度，m
    public float cylinderRadius = 1f; // 圆柱的底部半径，m

    public float totalDotAreaRatio = 0.04f; // 圆柱侧面积内所有小圆点的总面积比例


    public Vector3 cylinderBaseCenter = Vector3.zero; // 圆柱底部的中心位置设为原点
    public Vector3 cylinderTopCenter; // 圆柱顶部的中心位置

    private List<GameObject> dotPool = new List<GameObject>(); // 对象池
    private List<Vector3> savedDotPositions = new List<Vector3>(); // 保存生成的小圆点位置
    private bool isDisplay = true;
    void Start()
    {
        cylinderTopCenter = new Vector3(0f, 0f, cylinderHeight); // 圆柱顶部位置设为高度的顶点

        // 检查是否已保存圆点位置，如果没有，则生成新的
        if (savedDotPositions.Count == 0)
        {
            GenerateDots();
        }
        else
        {
            // 使用保存的位置生成小圆点
            PlaceDotsAtSavedPositions();
        }

    }

    void Update()
    {
    }

    void GenerateDots()
    {
        // 清空对象池
        foreach (var dot in dotPool)
        {
            Destroy(dot);
        }
        dotPool.Clear();

        // 清空已保存的圆点位置
        savedDotPositions.Clear();

        // 计算圆柱的侧面积
        float surfaceArea = 2 * Mathf.PI * cylinderRadius * cylinderHeight;

        // 计算所有小圆点的总面积
        float totalDotArea = surfaceArea * totalDotAreaRatio;

        // 计算每个小圆点的实际面积
        float dotArea = Mathf.PI * newDotRadius * newDotRadius;

        // 计算小圆点的数量
        int numDots = Mathf.RoundToInt(totalDotArea / dotArea);

        // initialRadius 是预制体的初始半径
        float initialRadius = 0.01f;

        // 获取小圆点预制体的 Transform 组件
        Transform dotTransform = dotPrefab.transform;

        // 计算新的缩放比例
        float scaleFactor = newDotRadius / initialRadius;

        // 设置小圆点预制体的缩放
        dotTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

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

            // 保存生成的小圆点位置
            savedDotPositions.Add(position);

            // 从对象池中获取或新建小圆点对象
            GameObject dot;
            if (dotPool.Count > i)
            {
                dot = dotPool[i];
                dot.SetActive(true);
                dot.transform.position = position;
            }
            else
            {
                dot = Instantiate(dotPrefab, position, Quaternion.identity);
                dot.transform.SetParent(transform); // 将其作为圆柱的子对象
                dotPool.Add(dot);
            }

            //GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity);
            //dot.transform.SetParent(transform); // 将其作为圆柱的子对象

            // 计算小圆点朝向圆柱表面的旋转
            Vector3 bposition = new Vector3(x, y, 0);
            Vector3 surfaceNormal = (bposition - cylinderBaseCenter).normalized;
            Quaternion rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);
            dot.transform.rotation = rotation;

        }
        // 多余的对象池对象隐藏
        for (int i = numDots; i < dotPool.Count; i++)
        {
            dotPool[i].SetActive(false);
        }
    }
    void PlaceDotsAtSavedPositions()
    {
        // 使用保存的圆点位置重新生成小圆点
        for (int i = 0; i < savedDotPositions.Count; i++)
        {
            Vector3 position = savedDotPositions[i];

            GameObject dot;
            if (dotPool.Count > i)
            {
                dot = dotPool[i];
                dot.SetActive(true);
                dot.transform.position = position;
            }
            else
            {
                dot = Instantiate(dotPrefab, position, Quaternion.identity);
                dot.transform.SetParent(transform);
                dotPool.Add(dot);
            }

            // 计算小圆点朝向圆柱表面的旋转
            Vector3 bposition = new Vector3(position.x, position.y, 0);
            Vector3 surfaceNormal = (bposition - cylinderBaseCenter).normalized;
            Quaternion rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);
            dot.transform.rotation = rotation;
        }

        // 隐藏多余的圆点
        for (int i = savedDotPositions.Count; i < dotPool.Count; i++)
        {
            dotPool[i].SetActive(false);
        }
    }


}
