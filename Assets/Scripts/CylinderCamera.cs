using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float captureInterval = 1f; // 拍摄间隔距离，m
    public RawImage preImageRawImage;
    public RawImage nextImageRawImage;

    private GameObject canvas;
    private Transform cameraTransform; // 主摄像机的Transform
    private float cameraCurrentZPosition = 0f; // 摄像机当前的Z轴位置
    private float nextCaptureZPosition = 0f; // 下次拍照的Z轴位置
    private Transform capturedImageTransform;
    private Transform userImageTransform;
    private Transform preImageTransform;
    private Transform nextImageTransform;
    private int captureImagesNumber = 0;
    private float cylinderHeight;// 圆柱的高度，m


    private List<(Texture2D, Vector3)> capturedImages; // 存储图片和位置的列表



    // Start is called before the first frame update
    void Start()
    {
        userCamera.transform.position = this.GetComponent<CylinderGenerator>().cylinderBaseCenter;// 相机初始位置设为圆柱底部中心
        cameraTransform = userCamera.transform; // 获取主摄像机的Transform
        cylinderHeight = this.GetComponent<CylinderGenerator>().cylinderHeight;
        cylinderTopCenter = new Vector3(0f, 0f, cylinderHeight); // 圆柱顶部位置设为高度的顶点

        captureCamera.enabled = false; // 初始化时禁用捕获摄像机
        capturedImages = new List<(Texture2D, Vector3)>();
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
                userImageRawImage.enabled = true;
                break;
            case Pattern.wobble:
                capturedImageRawImage.enabled = true;
                break;
            case Pattern.luminanceMixture:
                preImageRawImage.enabled = true;
                nextImageRawImage.enabled = true;
                CaptureImagesAtIntervalsSave();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (movementPattern)
        {
            case Pattern.continuous:
                ContinuousMoveCamera();
                break;
            case Pattern.wobble:
                WabbleMoveCamera();
                break;
            case Pattern.luminanceMixture:
                LuminanceMixtureMoveCamera();
                break;
        }
    }
    
    void ContinuousMoveCamera()
    {
        // 计算摄像机沿圆锥轴线移动的目标位置
        Vector3 direction = (cylinderTopCenter - cameraTransform.position).normalized;
        Vector3 targetPosition = cameraTransform.position + direction * cameraSpeed * Time.deltaTime;

        // 移动摄像机到目标位置
        //cameraTransform.position = targetPosition;
        //cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraSpeed * Time.deltaTime);
        cameraCurrentZPosition += captureInterval * Time.deltaTime;
        if (cameraCurrentZPosition > cylinderHeight)
        {
            cameraCurrentZPosition = cylinderHeight;
        }
        cameraTransform.position = new Vector3(0, 0, cameraCurrentZPosition);
        // 确保摄像机始终朝向圆锥顶点
        cameraTransform.LookAt(cylinderTopCenter);
    }
    void WabbleMoveCamera()
    {
        // 检查是否到了拍照的距离
        if (cameraCurrentZPosition >= nextCaptureZPosition)
        {
            Debug.Log("this is the number of the photo taken: " + captureImagesNumber);
            captureImagesNumber++;

            captureCamera.transform.position = cameraTransform.position;
            captureCamera.transform.rotation = cameraTransform.rotation;

            // 启用捕获摄像机
            //captureCamera.enabled = true;

            displayImage.texture = CaptureRenderTexture();

            // 禁用捕获摄像机
            //captureCamera.enabled = false;
            nextCaptureZPosition += captureInterval;
        }
        ContinuousMoveCamera();
    }

    void CaptureImagesAtIntervalsSave()
    {
        for (float z = 0; z <= cylinderHeight; z += captureInterval)
        {
            captureCamera.transform.position = new Vector3(0, 0, z);
 
            // 存储图片和对应的 位置
            capturedImages.Add((CaptureRenderTexture(), captureCamera.transform.position));
        }
    }

    void LuminanceMixtureMoveCamera()
    {
        var previousImage = capturedImages[captureImagesNumber]; // 获取前一个元素
        Texture2D previousTexture = previousImage.Item1; // 获取Texture2D
        Vector3 previousPosition = previousImage.Item2; // 获取位置

        var nextImage = capturedImages[captureImagesNumber+1]; // 获取后一个元素
        Texture2D nextTexture = nextImage.Item1; // 获取Texture2D
        Vector3 nextPosition = nextImage.Item2; // 获取位置

        //计算前一张图片到正在运动的相机的距离
        float preImageToCameraCurrentDistance = Vector3.Distance(previousPosition, cameraTransform.position);

        //计算前一张和后一张图片的辉度值
        float nextImageRatio = preImageToCameraCurrentDistance / captureInterval;
        float previousImageRatio = 1.0f - nextImageRatio;

        // 将修改后的颜色应用到 RawImage
        preImageRawImage.texture = previousTexture;
        preImageRawImage.color = new Color(preImageRawImage.color.r, preImageRawImage.color.g, preImageRawImage.color.b, previousImageRatio);
        nextImageRawImage.texture = nextTexture;
        nextImageRawImage.color = new Color(nextImageRawImage.color.r, nextImageRawImage.color.g, nextImageRawImage.color.b, nextImageRatio);
        // 设置父对象为 Canvas，并保持原始的本地位置、旋转和缩放
        preImageRawImage.transform.SetParent(canvas.transform, false);
        nextImageRawImage.transform.SetParent(canvas.transform, false);

        // 检查是否到了拍照的距离
        if (cameraCurrentZPosition >= nextPosition.z) 
        {
            captureImagesNumber++;
        }
        ContinuousMoveCamera();
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
}
