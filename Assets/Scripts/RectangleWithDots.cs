using UnityEngine;
using System.Collections.Generic;

public class RectangleWithDots : MonoBehaviour
{
    public float width = 25f; // 矩形短边宽度（米）
    public float length = 600f; // 矩形长边长度（米）
    public float density = 0.03f; // 点密度
    public float newDotRadius = 0.15f; // 小圆点目标半径（米）
    public GameObject dotPrefab; // 小圆点的预制体，必须包含 SpriteRenderer
    public Transform parentObject; // 父对象，用于挂载小白点

    private List<GameObject> dotPool = new List<GameObject>(); // 对象池
    private List<Vector3> precomputedPositions = new List<Vector3>(); // 预生成位置
    private int totalDots; // 点的总数

    void Start()
    {
        // 自动创建父对象
        if (parentObject == null)
        {
            GameObject parent = new GameObject("DotsParent");
            parentObject = parent.transform;
        }

        // 检查预制体是否设置
        if (dotPrefab == null)
        {
            Debug.LogError("dotPrefab 未设置，请在 Inspector 中设置一个预制体！");
            return;
        }

        // 计算点数量
        float surfaceArea = width * length;
        float totalDotArea = surfaceArea * density;
        float dotArea = Mathf.PI * newDotRadius * newDotRadius;
        totalDots = Mathf.RoundToInt(totalDotArea / dotArea);
        Debug.Log("Total Dots: " + totalDots);
        // 调整预制体的缩放
        float initialRadius = 0.01f; // 假设预制体的初始半径为 0.01m
        float scaleFactor = newDotRadius / initialRadius;
        dotPrefab.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        dotPrefab.transform.rotation = Quaternion.Euler(0, 90f, 0);
        // 预生成点位置
        PrecomputeDotPositions();

        // 初始化对象池
        InitializeObjectPool();

        // 生成点
        GenerateDots();
    }

    private void PrecomputeDotPositions()
    {
        precomputedPositions.Clear();
        for (int i = 0; i < totalDots; i++)
        {
            float y = Random.Range(-width / 2, width / 2); // Y 轴范围
            float z = Random.Range(-length / 2, length / 2); // Z 轴范围
            precomputedPositions.Add(new Vector3(6.45f, y, z)); // 点在 Y-Z 平面上
        }
    }

    private void InitializeObjectPool()
    {
        // 清空对象池
        foreach (var dot in dotPool)
        {
            Destroy(dot);
        }
        dotPool.Clear();

        // 创建对象池
        for (int i = 0; i < totalDots; i++)
        {
            GameObject dot = Instantiate(dotPrefab, Vector3.zero, dotPrefab.transform.rotation);
            dot.SetActive(false);
            dot.transform.SetParent(parentObject);
            dotPool.Add(dot);
        }
    }

    private void GenerateDots()
    {
        // 启用需要的点
        for (int i = 0; i < totalDots; i++)
        {
            GameObject dot = dotPool[i];
            dot.SetActive(true);
            dot.transform.position = precomputedPositions[i];
        }

        // 隐藏多余的点
        for (int i = totalDots; i < dotPool.Count; i++)
        {
            dotPool[i].SetActive(false);
        }
    }

    public void ClearDots()
    {
        // 隐藏所有点
        foreach (var dot in dotPool)
        {
            dot.SetActive(false);
        }
    }
}
