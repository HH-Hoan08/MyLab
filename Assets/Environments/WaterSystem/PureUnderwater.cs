 //using UnityEngine;
 //using System.Collections;

 //public class PureUnderwater : MonoBehaviour
 //{
 //    [Header("1. Tên GameObject Khối Nước (Name)")]
 //    [Tooltip("Nhập chính xác tên GameObject của khối nước trên Hierarchy")]
 //    public string surfaceWaterName = "WaterBox";    // Tên GameObject bơi nổi
 //    public string underwaterName = "Underwater";    // Tên GameObject lặn sâu

 //    [Header("2. Hiệu ứng Gợn Sóng Màn Hình (Shader Material)")]
 //    public Material underwaterMaterial;            // Gán Material Mat_UnderwaterRipple vào đây
 //    [Range(0f, 0.1f)]
 //    public float targetStrength = 0.02f;           // Độ méo gợn sóng khi lặn (mặc định 0.02)

 //    [Header("3. Audio Components")]
 //    public AudioSource audioSrc;
 //    public AudioLowPassFilter lowPass;

 //    [Header("4. Chỉnh Sương Mù (Fog)")]
 //    public Color underwaterFogColor = new Color(0.1f, 0.25f, 0.35f, 1f); 
 //    public float underwaterFogDensity = 0.08f; 

 //    [Header("5. Thời gian Delay & Hiệu ứng Âm thanh")]
 //    public float enterDelay = 0.2f; 
 //    public float exitDelay = 0.1f;  
 //    public float fadeDuration = 0.4f;

 //    [Header("6. Tùy Chỉnh Audio Low Pass Theo Độ Sâu")]
 //    public float waterSurfaceY = 43f;           // Tọa độ Y mặt nước
 //    public float maxDepth = 10f;              // Độ sâu tối đa
 //    public float surfaceCutoff = 3000f;       // Mức lọc khi ở nông
 //    public float deepCutoff = 800f;           // Mức lọc khi ở sâu

 //    private PlayerController player;
 //    private Coroutine waterCoroutine;
 //    private float maxVolume = 1f;

 //    // Lưu trữ cấu hình Fog mặc định ban đầu
 //    private bool defaultFog;
 //    private Color defaultFogColor;
 //    private float defaultFogDensity;
 //    private FogMode defaultFogMode;

 //    void Start()
 //    {
 //        player = GetComponent<PlayerController>();

 //        // Tự động tìm component trên Player/Camera nếu chưa kéo thủ công
 //        if (audioSrc == null) audioSrc = GetComponentInChildren<AudioSource>();
 //        if (lowPass == null) lowPass = GetComponentInChildren<AudioLowPassFilter>();

 //        if (audioSrc != null)
 //        {
 //            maxVolume = audioSrc.volume > 0 ? audioSrc.volume : 1f;
 //            audioSrc.volume = 0f;
 //        }

 //        if (lowPass != null) lowPass.enabled = false;

 //        // Reset độ méo gợn sóng về 0 khi vừa vào game (trên bờ)
 //        if (underwaterMaterial != null)
 //        {
 //            underwaterMaterial.SetFloat("_Strength", 0f);
 //        }

 //        // Lưu thiết lập Fog ban đầu của Scene
 //        defaultFog = RenderSettings.fog;
 //        defaultFogColor = RenderSettings.fogColor;
 //        defaultFogDensity = RenderSettings.fogDensity;
 //        defaultFogMode = RenderSettings.fogMode;
 //    }

 //    void Update()
 //    {
 //        // Cập nhật âm thanh đục/trong theo độ sâu lặn
 //        if (player != null && player.isUnderwater && lowPass != null && lowPass.enabled)
 //        {
 //            float currentDepth = waterSurfaceY - player.transform.position.y;
 //            float depthPercent = Mathf.Clamp01(currentDepth / maxDepth);

 //            lowPass.cutoffFrequency = Mathf.Lerp(surfaceCutoff, deepCutoff, depthPercent);
 //        }
 //    }

 //    private void OnTriggerEnter(Collider other)
 //    {
 //        Debug.Log("[PureUnderwater] Đã chạm vào Trigger: " + other.name);

 //        // 1. Chạm khối WATERBOX theo Name -> Bơi nổi
 //        if (other.name.Contains(surfaceWaterName))
 //        {
 //            Debug.Log("[PureUnderwater] -> Bật trạng thái bơi nổi (Swimming Surface)");
 //            if (player != null) player.isSwimmingSurface = true;
 //        }

 //        // 2. Chạm khối UNDERWATER theo Name -> Bật SÓNG MÀN HÌNH + ÂM THANH + FOG
 //        if (other.name.Contains(underwaterName))
 //        {
 //            Debug.Log("[PureUnderwater] -> Bật trạng thái lặn (Underwater Effect)");
 //            if (waterCoroutine != null) StopCoroutine(waterCoroutine);
 //            waterCoroutine = StartCoroutine(EnterUnderwaterRoutine());
 //        }
 //    }

 //    private void OnTriggerExit(Collider other)
 //    {
 //        // 1. Rời khối WATERBOX theo Name
 //        if (other.name.Contains(surfaceWaterName))
 //        {
 //            Debug.Log("[PureUnderwater] -> Thoát bơi nổi");
 //            if (player != null) player.isSwimmingSurface = false;
 //        }

 //        // 2. Rời khối UNDERWATER theo Name
 //        if (other.name.Contains(underwaterName))
 //        {
 //            Debug.Log("[PureUnderwater] -> Thoát lặn sâu");
 //            if (waterCoroutine != null) StopCoroutine(waterCoroutine);
 //            waterCoroutine = StartCoroutine(ExitUnderwaterRoutine());
 //        }
 //    }

 //    private IEnumerator EnterUnderwaterRoutine()
 //    {
 //        yield return new WaitForSeconds(enterDelay);

 //        if (player != null) 
 //        {
 //            player.isUnderwater = true;
 //            player.isSwimmingSurface = false; // Ưu tiên trạng thái lặn
 //        }

 //        if (lowPass != null) lowPass.enabled = true;

 //        // Bật gợn sóng méo màn hình trong Material
 //        if (underwaterMaterial != null)
 //        {
 //            underwaterMaterial.SetFloat("_Strength", targetStrength);
 //        }

 //        // Bật Fog môi trường nước
 //        RenderSettings.fog = true;
 //        RenderSettings.fogMode = FogMode.Exponential;
 //        RenderSettings.fogColor = underwaterFogColor;
 //        RenderSettings.fogDensity = underwaterFogDensity;

 //        // Fade in âm thanh dưới nước
 //        if (audioSrc != null)
 //        {
 //            if (!audioSrc.isPlaying) audioSrc.Play();

 //            float timer = 0f;
 //            float startVol = audioSrc.volume;

 //            while (timer < fadeDuration)
 //            {
 //                timer += Time.deltaTime;
 //                audioSrc.volume = Mathf.Lerp(startVol, maxVolume, timer / fadeDuration);
 //                yield return null;
 //            }

 //            audioSrc.volume = maxVolume;
 //        }
 //    }

 //    private IEnumerator ExitUnderwaterRoutine()
 //    {
 //        yield return new WaitForSeconds(exitDelay);

 //        if (player != null) player.isUnderwater = false;

 //        // Tắt độ méo màn hình về 0
 //        if (underwaterMaterial != null)
 //        {
 //            underwaterMaterial.SetFloat("_Strength", 0f);
 //        }

 //        // Khôi phục Fog môi trường ban đầu
 //        RenderSettings.fog = defaultFog;
 //        RenderSettings.fogColor = defaultFogColor;
 //        RenderSettings.fogDensity = defaultFogDensity;
 //        RenderSettings.fogMode = defaultFogMode;

 //        // Fade out âm thanh dưới nước
 //        if (audioSrc != null)
 //        {
 //            float timer = 0f;
 //            float startVol = audioSrc.volume;

 //            while (timer < fadeDuration)
 //            {
 //                timer += Time.deltaTime;
 //                audioSrc.volume = Mathf.Lerp(startVol, 0f, timer / fadeDuration);
 //                yield return null;
 //            }

 //            audioSrc.volume = 0f;
 //            audioSrc.Stop();
 //        }

 //        if (lowPass != null) lowPass.enabled = false;
 //    }

 //    private void OnDisable()
 //    {
 //        // Tắt méo Material khi bấm Stop Game để tránh bị lưu biến trong Editor
 //        if (underwaterMaterial != null)
 //        {
 //            underwaterMaterial.SetFloat("_Strength", 0f);
 //        }
 //    }
 //}