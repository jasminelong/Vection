using System.Collections.Generic;
using UnityEngine;

public class TexturePool : MonoBehaviour
{
    public static TexturePool Instance; // シングルトンインスタンス

    public int initialPoolSize = 5; // 初期プールサイズ
    private Queue<RenderTexture> renderTexturePool = new Queue<RenderTexture>();
    private Queue<Texture2D> texture2DPool = new Queue<Texture2D>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    // プールの初期化
    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            RenderTexture rt = new RenderTexture(1, 1, 24); // 必要に応じてRenderTextureのサイズを初期化
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGB24, false); // 必要に応じてTexture2Dのサイズを初期化
            renderTexturePool.Enqueue(rt);
            texture2DPool.Enqueue(tex);
        }
    }

    // プールからRenderTextureを取得
    public RenderTexture GetRenderTextureFromPool(int width, int height)
    {
        if (renderTexturePool.Count > 0)
        {
            RenderTexture rt = renderTexturePool.Dequeue();
            rt.width = width;
            rt.height = height;
            return rt;
        }
        else
        {
            return new RenderTexture(width, height, 24);
        }
    }

    // RenderTextureをプールに戻す
    public void ReturnRenderTextureToPool(RenderTexture rt)
    {
        // 次回使用のためにRenderTextureの内容をクリア
        rt.Release();
        renderTexturePool.Enqueue(rt);
    }

    // プールからTexture2Dを取得
    public Texture2D GetTexture2DFromPool(int width, int height, TextureFormat format)
    {
        if (texture2DPool.Count > 0)
        {
            Texture2D tex = texture2DPool.Dequeue();
            tex.Reinitialize(width, height, format, false);
            return tex;
        }
        else
        {
            return new Texture2D(width, height, format, false);
        }
    }

    // Texture2Dをプールに戻す
    public void ReturnTexture2DToPool(Texture2D tex)
    {
        // 次回使用のためにTexture2Dの内容をクリア
        tex.SetPixels(new Color[tex.width * tex.height]);
        tex.Apply();
        texture2DPool.Enqueue(tex);
    }
}
