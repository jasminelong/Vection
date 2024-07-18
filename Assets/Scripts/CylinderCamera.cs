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
    public Pattern movementPattern; // イメージの提示パターン // 图像提示的模式
    public GameObject moveSphere; // 連続運動のカメラ // 连续运动的摄像机
    public Camera captureCamera; // 一定の距離ごとに写真を撮るためのカメラ // 用于间隔一定距离拍照的摄像机
    public Vector3 cylinderTopCenter; // 円柱の頂点の中心位置 // 圆柱顶部的中心位置
    public float cameraSpeed = 1f; // カメラが円柱の軸に沿って移動する速度 (m/s) // 摄像机沿圆柱轴线移动的速度，m/s
    public float fps = 60f; // 他のfps // 其他的fps

    private float trialTime = 3 * 60 * 1000;//实验的总时间
    private float captureIntervalDistance; // 撮影間隔の距離 (m) // 拍摄间隔距离，m
    private GameObject canvas;
    private Transform moveSphereTransform; // メインカメラのTransform // 主摄像机的Transform
    private Transform displayImageTransform;
    private Transform preImageTransform;
    private Transform nextImageTransform;
    private RawImage displayImageRawImage;// 撮影した画像を表示するためのUIコンポーネント // 用于显示拍摄图像的UI组件
    private RawImage preImageRawImage;// 撮影した画像を表示するためのUIコンポーネント // 用于显示拍摄图像的UI组件
    private RawImage nextImageRawImage;// 撮影した画像を表示するためのUIコンポーネント // 用于显示拍摄图像的UI组件
    private RectTransform capturedImageRect;
    private int capturedImageWidth;
    private int capturedImageHeight;
    private float cylinderHeight; // 円柱の高さ (m) // 圆柱的高度，m

    private List<(Texture2D, Vector3)> capturedImages; // 画像と位置を格納するリスト // 存储图片和位置的列表

    public float updateInterval; // 更新間隔 (秒) // 更新间隔，单位秒
    private float updateTimer = 0f;

    // データ保存用のフィールド // 保存数据用的字段
    // 現在のフレーム数と時間を取得 // 获取当前帧数和时间
    public int frameNum = 0;
    public string participantName;
    private string experimentalCondition;
    public int trialNumber;

    private List<string> data = new List<string>();
    private float startTime;
    private bool vectionResponse = false;
    private string folderName = "ExperimentData"; // サブフォルダ名 // 子文件夹名称
    private float timeMs; // 現在までの経過時間 // 运行到现在的时间
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;

        // 垂直同期を無効にする // 关闭垂直同步
        QualitySettings.vSyncCount = 0;
        // 目標フレームレートを60フレーム/秒に設定 // 设置目标帧率为60帧每秒
        Time.fixedDeltaTime = 1.0f / 60.0f;

        moveSphere.transform.position = this.GetComponent<CylinderGenerator>().cylinderBaseCenter; // カメラの初期位置を円柱の底部中心に設定 // 相机初始位置设为圆柱底部中心
        moveSphereTransform = moveSphere.transform; // メインカメラのTransformを取得 // 获取主摄像机的Transform
        cylinderHeight = this.GetComponent<CylinderGenerator>().cylinderHeight;
        cylinderTopCenter = new Vector3(0f, 0f, cylinderHeight); // 円柱の頂点位置を高さの頂点に設定 // 圆柱顶部位置设为高度的顶点

        captureCamera.enabled = false; // 初期状態でキャプチャカメラを無効にする // 初始化时禁用捕获摄像机
        capturedImages = new List<(Texture2D, Vector3)>();

        updateInterval = 1 / fps; // 各フレームの表示間隔を計算 // 计算每一帧显示的间隔时间
        captureIntervalDistance = cameraSpeed / fps; // 各フレームの間隔距離を計算 // 计算每帧之间的间隔距离

        GetRawImage();

        switch (movementPattern)
        {
            case Pattern.continuous:
            case Pattern.wobble:
                data.Add("FrameNum, Time [ms], Vection Response (0:no, 1: yes )");
                displayImageRawImage.enabled = true;
                CaptureImagesAtIntervalsSave();
                break;
            case Pattern.luminanceMixture:
                data.Add("FrondFrameNum, FrondFrameLuminance, BackFrameNum, BackFrameLuminance, Time [ms], Vection Response (0:no, 1: yes )");
                preImageRawImage.enabled = true;
                nextImageRawImage.enabled = true;
                CaptureImagesAtIntervalsSave();
                break;
        }

        experimentalCondition = movementPattern.ToString() + "_"
                                                 + "cameraSpeed" + cameraSpeed.ToString() + "_"
                                                 + "fps" + fps.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeMs = (Time.time - startTime) * 1000;
        // キーの状態をチェック // 检测按键状态
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
            case Pattern.wobble:
                Wabble();
                break;
            case Pattern.luminanceMixture:
                LuminanceMixture();
                break;
        }
        //MoveCamera();
        MoveSphere();
    }

    void GetRawImage()
    {
        // Canvas内で指定された名前の子オブジェクトを検索 // 在 Canvas 中查找指定名称的子对象
        canvas = GameObject.Find("Canvas");
        displayImageTransform = canvas.transform.Find("DisplayImage");
        preImageTransform = canvas.transform.Find("PreImageRawImage");
        nextImageTransform = canvas.transform.Find("NextImageRawImage");

        // 子オブジェクトのRawImageコンポーネントを取得 // 获取子对象的 RawImage 组件
         displayImageRawImage = displayImageTransform.GetComponent<RawImage>();
         preImageRawImage = preImageTransform.GetComponent<RawImage>();
         nextImageRawImage = nextImageTransform.GetComponent<RawImage>();

        // RawImageコンポーネントを無効にする // 禁用 RawImage 组件
        displayImageRawImage.enabled = false;
        preImageRawImage.enabled = false;
        nextImageRawImage.enabled = false;

        // RawImageの幅と高さを取得 // 获取 RawImage 的宽高
        capturedImageRect = displayImageTransform.GetComponent<RectTransform>();
        capturedImageWidth = (int)capturedImageRect.rect.width;
        capturedImageHeight = (int)capturedImageRect.rect.height;
    }

 
    void MoveSphere()
    {
        // カメラが円柱の軸に沿って移動する目標位置を計算 // 计算摄像机沿圆锥轴线移动的目标位置
        Vector3 direction = (cylinderTopCenter - moveSphereTransform.position).normalized;
        Vector3 targetPosition = moveSphereTransform.position + direction * cameraSpeed * Time.deltaTime;
        if (startTime >= trialTime)
        {
            QuitGame();
        }
        // カメラを目標位置に移動 // 移动摄像机到目标位置
        moveSphereTransform.position = targetPosition;

        // カメラを常に円柱の頂点に向ける // 确保摄像机始终朝向圆锥顶点
        moveSphereTransform.LookAt(cylinderTopCenter);
    }
    void Wabble()
    {
        var Image = capturedImages[frameNum]; // 画像を取得 // 获取一个元素
        Texture2D Texture = Image.Item1; // Texture2Dを取得 // 获取Texture2D

        if (Mathf.Abs(timeMs - frameNum * updateInterval * 1000) < 0.01f)
        {
            displayImageRawImage.texture = Texture;
            frameNum++;
        }

        // データを記録 // 记录数据,这里是为了记录数据从1开始，所以用的frameNum而不是frameNum-1,因为list的下标是从0开始的
        data.Add($"{frameNum}, {timeMs:F4}, {(vectionResponse ? 1 : 0)}");
    }

    void CaptureImagesAtIntervalsSave()
    {
        for (float z = 0; z <= cylinderHeight; z += captureIntervalDistance)
        {
            captureCamera.transform.position = new Vector3(0, 0, z);

            // 画像と対応する位置を保存 // 保存图像及其位置
            capturedImages.Add((CaptureRenderTexture(), captureCamera.transform.position));
        }
    }

    void LuminanceMixture()
    {
        if (frameNum  < capturedImages.Count - 1)
        {
            // 写真を撮る距離に達したかをチェック // 检查是否到了拍照的距离
            if (Mathf.Abs(timeMs - (frameNum+1) * updateInterval * 1000) < 0.01f )
            {
                frameNum++;
            }
            var previousImage = capturedImages[frameNum]; // 手前の画像を取得 // 获取前一个元素
            Texture2D previousTexture = previousImage.Item1; // Texture2Dを取得 // 获取Texture2D
            Vector3 previousPosition = previousImage.Item2; // 位置を取得 // 获取位置

            var nextImage = capturedImages[frameNum + 1]; // 次の画像を取得 // 获取后一个元素
            Texture2D nextTexture = nextImage.Item1; // Texture2Dを取得 // 获取Texture2D

            //  手前の画像から現在のカメラまでの距離を計算 // 计算前一张图片到正在运动的相机的距离
            float preImageToCameraCurrentDistance = Vector3.Distance(previousPosition, moveSphereTransform.position);

            //  手前の画像と次の画像の輝度値を計算 // 计算前一张和后一张图片的辉度值
            float nextRatio = preImageToCameraCurrentDistance / captureIntervalDistance;
            float nextImageRatio = nextRatio > 1.0f ? 1.0f : nextRatio;
            float previousImageRatio = 1.0f - nextImageRatio;

            // 変更された色をRawImageに適用 // 将修改后的颜色应用到 RawImage
            preImageRawImage.texture = previousTexture;
            preImageRawImage.color = new Color(preImageRawImage.color.r, preImageRawImage.color.g, preImageRawImage.color.b, previousImageRatio);

            nextImageRawImage.texture = nextTexture;
            nextImageRawImage.color = new Color(nextImageRawImage.color.r, nextImageRawImage.color.g, nextImageRawImage.color.b, nextImageRatio);
            // Canvasに親オブジェクトを設定し、元のローカル位置、回転、およびスケールを保持 // 设置父对象为 Canvas，并保持原始的本地位置、旋转和缩放
            preImageRawImage.transform.SetParent(canvas.transform, false);
            nextImageRawImage.transform.SetParent(canvas.transform, false);


            // データを記録 // 记录数据
            data.Add($"{frameNum+1}, {previousImageRatio:F5},{frameNum + 2}, {nextImageRatio:F5},{timeMs:F5}, {(vectionResponse ? 1 : 0)}");
        }
    }
    Texture2D CaptureRenderTexture()
    {
        // RenderTextureを作成し、キャプチャカメラのターゲットとして設定 // 创建一个RenderTexture并将其设置为捕获摄像机的目标
        RenderTexture rt = TexturePool.Instance.GetRenderTextureFromPool(capturedImageWidth, capturedImageHeight);
        captureCamera.targetTexture = rt;

        // 画像をレンダリング // 渲染图像
        captureCamera.Render();

        // RenderTextureをアクティブ化 // 激活 RenderTexture
        RenderTexture.active = rt;

        // 新しいTexture2Dを作成 // 创建一个新的 Texture2D
        Texture2D capturedImage = TexturePool.Instance.GetTexture2DFromPool(rt.width, rt.height, TextureFormat.RGB24);

        // RenderTextureからピクセルデータをTexture2Dに読み込む // 从 RenderTexture 中读取像素数据到 Texture2D
        capturedImage.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        capturedImage.Apply();

        // 解放 // 释放
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        TexturePool.Instance.ReturnRenderTextureToPool(rt);
        return capturedImage;
    }
    void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // エディターでのプレイモードを停止 // 在编辑器中停止播放模式
        #else
                    Application.Quit(); // アプリケーションでアプリを終了 // 在应用程序中退出应用
        #endif
    }

    void OnApplicationQuit()
    {
        // 現在の日付を取得 // 获取当前日期
        string date = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        // ファイル名を構築 // 构建文件名
        string fileName = $"{date}_{experimentalCondition}_{participantName}_trialNumber{trialNumber}.csv";

        // ファイルを保存（Application.dataPath：現在のプロジェクトのAssetsフォルダのパスを示す） // 保存文件（Application.dataPath：表示当前项目的Assets文件夹的路径）
        string filePath = Path.Combine(Application.dataPath, folderName, fileName);
        File.WriteAllLines(filePath, data);

        Debug.Log($"Data saved to {filePath}");
    }
}
