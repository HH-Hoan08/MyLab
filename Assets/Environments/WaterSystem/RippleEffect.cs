using System.Collections;
using UnityEngine;

public class RippleEffect : MonoBehaviour
{
    public int TextureSize = 512;
    public RenderTexture ObjectsRT;
    private RenderTexture CurrRT, PrevRT, TempRT;
    public Shader RippleShader, AddShader;
    private Material RippleMat, AddMat;

    private Coroutine rippleCoroutine;
    private bool isSimulating = true;

    void Start()
    {
        // Khởi tạo RenderTextures và Materials
        CurrRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat);
        PrevRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat);
        TempRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat);

        RippleMat = new Material(RippleShader);
        AddMat = new Material(AddShader);

        // Gán kết quả tính toán vào Material của mặt nước
        GetComponent<Renderer>().material.SetTexture("_RippleTex", CurrRT);

        StartSimulation();
    }

    public void StartSimulation()
    {
        if (rippleCoroutine == null)
        {
            isSimulating = true;
            rippleCoroutine = StartCoroutine(RipplesRoutine());
        }
    }

    public void StopSimulation()
    {
        isSimulating = false;
        if (rippleCoroutine != null)
        {
            StopCoroutine(rippleCoroutine);
            rippleCoroutine = null;
        }
    }

    // Dùng vòng lặp while(true) thay vì gọi đệ quy StartCoroutine để tối ưu hiệu năng
    private IEnumerator RipplesRoutine()
    {
        while (isSimulating)
        {
            // Trộn ObjectsRT vào CurrentRT
            AddMat.SetTexture("_ObjectsRT", ObjectsRT);
            AddMat.SetTexture("_CurrentRT", CurrRT);
            Graphics.Blit(null, TempRT, AddMat);

            RenderTexture rt0 = TempRT;
            TempRT = CurrRT;
            CurrRT = rt0;

            // Tính toán lan truyền sóng
            RippleMat.SetTexture("_PrevRT", PrevRT);
            RippleMat.SetTexture("_CurrentRT", CurrRT);
            Graphics.Blit(null, TempRT, RippleMat);
            Graphics.Blit(TempRT, PrevRT);

            // Hoán đổi buffer
            RenderTexture rt = PrevRT;
            PrevRT = CurrRT;
            CurrRT = rt;

            yield return null;
        }
    }

    private void OnDestroy()
    {
        // Giải phóng RenderTexture khi destroy scene/object
        if (CurrRT != null) CurrRT.Release();
        if (PrevRT != null) PrevRT.Release();
        if (TempRT != null) TempRT.Release();
    }
}