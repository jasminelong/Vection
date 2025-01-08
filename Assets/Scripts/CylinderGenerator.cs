using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode] // 支持在编辑模式运行
public class CylinderGenerator : MonoBehaviour
{
    public GameObject dotPrefab; // 小圆点的预制体
    public float newDotRadius = 0.01f; // 小圆点半径（米）
    public float cylinderHeight = 20f; // 圆柱的高度（米）
    public float cylinderRadius = 1f; // 圆柱的底部半径（米）
    public float totalDotAreaRatio = 0.04f; // 小圆点总面积与圆柱侧面积的比例
    public Vector3 cylinderBaseCenter = Vector3.zero; // 圆柱底部中心
    public Vector3 cylinderTopCenter; // 圆柱顶部中心
    private List<GameObject> dotPool = new List<GameObject>(); // 对象池
    [SerializeField] private List<Vector3> savedDotPositions = new List<Vector3>(); // 保存的小圆点位置
    private bool hasGenerated = false; // 防止重复生成

    void Start()
    {
        // 确保顶部中心的位置正确
        cylinderTopCenter = cylinderBaseCenter + new Vector3(0f, 0f, cylinderHeight);

        // 如果没有保存的圆点位置，生成新的
        if (savedDotPositions.Count == 0 && !Application.isPlaying)
        {
            GenerateDots();
        }
        else
        {
            PlaceDotsAtSavedPositions();
        }
    }

    private void OnValidate()
    {
        // 当参数更改时，在编辑模式自动更新圆点
        if (!Application.isPlaying)
        {
            GenerateDots();
        }
    }

    void GenerateDots()
    {
        if (hasGenerated) return;

#if UNITY_EDITOR
        Undo.RegisterFullObjectHierarchyUndo(gameObject, "Generate Dots"); // 记录场景更改
#endif

        // 清空对象池和保存的圆点位置
        foreach (var dot in dotPool)
        {
            DestroyImmediate(dot);
        }
        dotPool.Clear();
        savedDotPositions.Clear();

        // 计算圆柱侧面积
        float surfaceArea = 2 * Mathf.PI * cylinderRadius * cylinderHeight;

        // 计算所有小圆点的总面积
        float totalDotArea = surfaceArea * totalDotAreaRatio;

        // 计算每个小圆点的面积
        float dotArea = Mathf.PI * newDotRadius * newDotRadius;

        // 计算小圆点的数量
        int numDots = Mathf.RoundToInt(totalDotArea / dotArea);

        // 计算缩放比例
        float initialRadius = 0.01f;
        float scaleFactor = newDotRadius / initialRadius;

        // 生成小圆点
        for (int i = 0; i < numDots; i++)
        {
            float theta = Random.Range(0f, Mathf.PI * 2f); // 随机角度
            float height = Random.Range(0f, cylinderHeight); // 随机高度

            float x = cylinderRadius * Mathf.Cos(theta);
            float y = cylinderRadius * Mathf.Sin(theta);
            Vector3 position = new Vector3(x, y, height);

            // 保存位置
            savedDotPositions.Add(position);

            // 创建小圆点
            GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity);
            dot.transform.SetParent(transform);
            dot.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            // 计算朝向
            Vector3 surfaceNormal = new Vector3(x, y, 0).normalized;
            dot.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);

            dotPool.Add(dot);
        }

        hasGenerated = true;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this); // 标记对象已更改
#endif
    }

    void PlaceDotsAtSavedPositions()
    {
        foreach (var dot in dotPool)
        {
            DestroyImmediate(dot);
        }
        dotPool.Clear();

        float initialRadius = 0.01f;
        float scaleFactor = newDotRadius / initialRadius;

        for (int i = 0; i < savedDotPositions.Count; i++)
        {
            Vector3 position = savedDotPositions[i];
            GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity);
            dot.transform.SetParent(transform);
            dot.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            Vector3 surfaceNormal = new Vector3(position.x, position.y, 0).normalized;
            dot.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);

            dotPool.Add(dot);
        }
    }

    public void ClearDots()
    {
        foreach (var dot in dotPool)
        {
            DestroyImmediate(dot);
        }
        dotPool.Clear();
        savedDotPositions.Clear();
    }
}
