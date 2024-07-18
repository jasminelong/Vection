using System.Collections.Generic;
using UnityEngine;

public class TexturePool : MonoBehaviour
{
    public static TexturePool Instance; // �V���O���g���C���X�^���X

    public int initialPoolSize = 5; // �����v�[���T�C�Y
    private Queue<RenderTexture> renderTexturePool = new Queue<RenderTexture>();
    private Queue<Texture2D> texture2DPool = new Queue<Texture2D>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    // �v�[���̏�����
    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            RenderTexture rt = new RenderTexture(1, 1, 24); // �K�v�ɉ�����RenderTexture�̃T�C�Y��������
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGB24, false); // �K�v�ɉ�����Texture2D�̃T�C�Y��������
            renderTexturePool.Enqueue(rt);
            texture2DPool.Enqueue(tex);
        }
    }

    // �v�[������RenderTexture���擾
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

    // RenderTexture���v�[���ɖ߂�
    public void ReturnRenderTextureToPool(RenderTexture rt)
    {
        // ����g�p�̂��߂�RenderTexture�̓��e���N���A
        rt.Release();
        renderTexturePool.Enqueue(rt);
    }

    // �v�[������Texture2D���擾
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

    // Texture2D���v�[���ɖ߂�
    public void ReturnTexture2DToPool(Texture2D tex)
    {
        // ����g�p�̂��߂�Texture2D�̓��e���N���A
        tex.SetPixels(new Color[tex.width * tex.height]);
        tex.Apply();
        texture2DPool.Enqueue(tex);
    }
}
