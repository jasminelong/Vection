using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class CylinderCamera : MonoBehaviour
{
    public enum Pattern
    {
        continuous,
        wobble,
        luminanceMixture
    }
    public Pattern movementPattern;//图像提示的模式
    public Camera userCamera;//连续运动的摄像机
    public Camera captureCamera; // 用于间隔一定距离拍照的摄像机
    public Vector3 cylinderTopCenter; // 圆柱顶部的中心位置
    public float cameraSpeed = 1f; // 摄像机沿圆柱轴线移动的速度，m/s
    public RawImage displayImage; // 用于显示拍摄图像的UI组件
    public float fps = 60f;//其他的fps
    public RawImage preImageRawImage;
    public RawImage nextImageRawImage;

    private float captureIntervalDistance; // 拍摄间隔距离，m
    private GameObject canvas;
    private Transform cameraTransform; // 主摄像机的Transform
    private Transform capturedImageTransform;
    private Transform userImageTransform;
    private Transform preImageTransform;
    private Transform nextImageTransform;
    private int captureImagesNumber = 0;
    private float cylinderHeight;// 圆柱的高度，m


    private List<(Texture2D, Vector3)> capturedImages; // 存储图片和位置的列表

    public float updateInterval; // 更新间隔，单位秒
    private float updateTimer = 0f;
 
    //保存数据用的字段
    // 获取当前帧数和时间
    public int frameNum=0;
    public string participantName;
    private string experimentalCondition;
    public int trialNumber;

    private List<string> data = new List<string>();
    private float startTime;
    private bool vectionResponse = false;
    private string folderName = "ExperimentData"; // 子文件夹名称
    private float timeMs ;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        // 关闭垂直同步
        QualitySettings.vSyncCount = 0;
        // 设置目标帧率为60帧每秒
        Application.targetFrameRate = 60;

        userCamera.transform.position = this.GetComponent<CylinderGenerator>().cylinderBaseCenter;// 相机初始位置设为圆柱底部中心
        cameraTransform = userCamera.transform; // 获取主摄像机的Transform
        cylinderHeight = this.GetComponent<CylinderGenerator>().cylinderHeight;
        cylinderTopCenter = new Vector3(0f, 0f, cylinderHeight); // 圆柱顶部位置设为高度的顶点

        captureCamera.enabled = false; // 初始化时禁用捕获摄像机
        capturedImages = new List<(Texture2D, Vector3)>();

        updateInterval = 1 / fps;//计算每一帧显示的间隔时间

        captureIntervalDistance = cameraSpeed / fps;//计算每帧直接的间隔距离

        // 在 Canvas 中查找指定名称的子对象
        canvas = GameObject.Find("Canvas");
        userImageTransform = canvas.transform.Find("UserImage");
        capturedImageTransform = canvas.transform.Find("CapturedImage");
        preImageTransform = canvas.transform.Find("preImageRawImage");
        nextImageTransform = canvas.transform.Find("nextImageRawImage");

        // 获取子对象的 RawImage 组件
        RawImage userImageRawImage = userImageTransform.GetComponent<RawImage>();
        RawImage capturedImageRawImage = capturedImageTransform.GetComponent<RawImage>();
        RawImage preImageRawImage = preImageTransform.GetComponent<RawImage>();
        RawImage nextImageRawImage = nextImageTransform.GetComponent<RawImage>();

        // 禁用 RawImage 组件
        userImageRawImage.enabled = false;
        capturedImageRawImage.enabled = false;
        preImageRawImage.enabled = false;
        nextImageRawImage.enabled = false;

        switch (movementPattern)
        {
            case Pattern.continuous:
                // 添加数据表头
                data.Add("FrameNum, Time [ms], Vection Response (0:no, 1: yes )");
                userImageRawImage.enabled = true;
                break;
            case Pattern.wobble:
                // 添加数据表头
                data.Add("FrameNum, Time [ms], Vection Response (0:no, 1: yes )");
                capturedImageRawImage.enabled = true;
                displayImage.texture = CaptureRenderTexture();
                frameNum++;
                break;
            case Pattern.luminanceMixture:
                // 添加数据表头
                data.Add("FrondFrameNum, FrondFrameLuminance, BackFrameNum, BackFrameLuminance, Time [ms], Vection Response (0:no, 1: yes )");
                preImageRawImage.enabled = true;
                nextImageRawImage.enabled = true;
                CaptureImagesAtIntervalsSave();
                break;
        }

        startTime = Time.time;
        experimentalCondition = movementPattern.ToString() + "_"
                                                 + "cameraSpeed" + cameraSpeed.ToString() + "_"
                                                 + "fps" + fps.ToString()
                                                 ;
    }

    // Update is called once per frame
    void Update()
    {
        timeMs = (Time.time - startTime) * 1000;
        // 检测按键状态
        if (Input.GetKey(KeyCode.Space))
        {
            vectionResponse = true;
        }
        else
        {
            vectionResponse = false;
        }
        switch (movementPattern)
        {
            case Pattern.continuous:
                frameNum = Time.frameCount;
                Continuous();
                // 记录数据
                data.Add($"{frameNum}, {timeMs:F4}, {(vectionResponse ? 1 : 0)}");
                break;
            case Pattern.wobble:
                Wabble();
                // 记录数据
                data.Add($"{frameNum}, {timeMs:F4}, {(vectionResponse ? 1 : 0)}");
                break;
            case Pattern.luminanceMixture:
                LuminanceMixture();

                break;
        }
    }
    void Continuous()
    {
        MoveCamera();
    }
    void MoveCamera()
    {
        // 计算摄像机沿圆锥轴线移动的目标位置
        Vector3 direction = (cylinderTopCenter - cameraTransform.position).normalized;
        Vector3 targetPosition = cameraTransform.position + direction * cameraSpeed * Time.deltaTime;
        float distanceToTarget = Vector3.Distance(cameraTransform.position, cylinderTopCenter);
        if (distanceToTarget <= 0.1f)
        {
            QuitGame();
        }
        // 移动摄像机到目标位置
        cameraTransform.position = targetPosition;
      
        //cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraSpeed * Time.deltaTime);
        // 确保摄像机始终朝向圆锥顶点
        cameraTransform.LookAt(cylinderTopCenter);
    }
    void Wabble()
    {
        MoveCamera();
        

        // 控制帧率更新
        if (timeMs >= frameNum*updateInterval * 1000)
        {
            captureCamera.transform.position = cameraTransform.position;
            captureCamera.transform.rotation = cameraTransform.rotation;

            // 更新Raw Image显示的Texture
            displayImage.texture = CaptureRenderTexture();
            
            // 重置更新计时器
            updateTimer = 0f;

            //记录帧数
            frameNum++;
        }
    }

    void CaptureImagesAtIntervalsSave()
    {
        for (float z = 0; z <= cylinderHeight; z += captureIntervalDistance)
        {
            captureCamera.transform.position = new Vector3(0, 0, z);
 
            // 存储图片和对应的 位置
            capturedImages.Add((CaptureRenderTexture(), captureCamera.transform.position));
        }
    }

    void LuminanceMixture()
    {
        MoveCamera();
        var previousImage = capturedImages[captureImagesNumber]; // 获取前一个元素
        Texture2D previousTexture = previousImage.Item1; // 获取Texture2D
        Vector3 previousPosition = previousImage.Item2; // 获取位置

        var nextImage = capturedImages[captureImagesNumber+1]; // 获取后一个元素
        Texture2D nextTexture = nextImage.Item1; // 获取Texture2D
        Vector3 nextPosition = nextImage.Item2; // 获取位置

        //计算前一张图片到正在运动的相机的距离
        float preImageToCameraCurrentDistance = Vector3.Distance(previousPosition, cameraTransform.position);

        //计算前一张和后一张图片的辉度值
        float nextImageRatio = preImageToCameraCurrentDistance / captureIntervalDistance;
        float previousImageRatio = 1.0f - nextImageRatio;

        // 将修改后的颜色应用到 RawImage
        preImageRawImage.texture = previousTexture;
        preImageRawImage.color = new Color(preImageRawImage.color.r, preImageRawImage.color.g, preImageRawImage.color.b, previousImageRatio);
        nextImageRawImage.texture = nextTexture;
        nextImageRawImage.color = new Color(nextImageRawImage.color.r, nextImageRawImage.color.g, nextImageRawImage.color.b, nextImageRatio);
        // 设置父对象为 Canvas，并保持原始的本地位置、旋转和缩放
        preImageRawImage.transform.SetParent(canvas.transform, false);
        nextImageRawImage.transform.SetParent(canvas.transform, false);

        // 记录数据
        data.Add($"{captureImagesNumber}, {previousImageRatio},{captureImagesNumber + 1}, {nextImageRatio},{timeMs:F4}, {(vectionResponse ? 1 : 0)}");

        // 检查是否到了拍照的距离
        if (cameraTransform.position.z >= nextPosition.z) 
        {
            captureImagesNumber++;
        }
       

    }
    Texture2D CaptureRenderTexture()
    {
        // 获取 RawImage 的宽高
        RectTransform capturedImageRect = capturedImageTransform.GetComponent<RectTransform>();
        int width = (int)capturedImageRect.rect.width;
        int height = (int)capturedImageRect.rect.height;

        // 创建一个RenderTexture并将其设置为捕获摄像机的目标
        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;

        // 渲染图像
        captureCamera.Render();
        
        // 激活 RenderTexture
        RenderTexture.active = rt;

        // 创建一个新的 Texture2D
        Texture2D capturedImage = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

        // 从 RenderTexture 中读取像素数据到 Texture2D
        capturedImage.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        capturedImage.Apply();

        // 释放
        captureCamera.targetTexture = null;
        RenderTexture.active = null; 
        Destroy(rt);

        return capturedImage;
    }
    void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // 在编辑器中停止播放模式
        #else
                Application.Quit(); // 在应用程序中退出应用
        #endif
    }

    void OnApplicationQuit()
    {
        // 获取当前日期
        string date = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        // 构建文件名
        string fileName = $"{date}_{experimentalCondition}_{participantName}_trialNumber{trialNumber}.csv";

        // 保存文件（Application.dataPath：表示当前项目的Assets文件夹的路径）
        string filePath = Path.Combine(Application.dataPath, folderName, fileName);
        File.WriteAllLines(filePath, data);

        Debug.Log($"Data saved to {filePath}");
    }
}
