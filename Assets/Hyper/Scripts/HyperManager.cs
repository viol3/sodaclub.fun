using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HyperManager : MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void InitSDK();

    [DllImport("__Internal")]
    private static extern void ShowAd();
#elif UNITY_ANDROID
    [DllImport("hyperads")] // .so dosyasının adı "libhyperads.so" olmalı
    private static extern void InitSDK();

    [DllImport("hyperads")]
    private static extern void ShowAd();
#else
    private static void InitSDK() { Debug.Log("InitSDK stub"); }
    private static void ShowAd() { Debug.Log("ShowAd stub"); }
#endif

    public static HyperManager Instance;

    private bool _inited = false;
    public event System.Action<bool> OnInitPhaseFinished;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        try
        {
            InitSDK();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("InitSDK call failed: " + ex.Message);
            OnInitPhaseFinished?.Invoke(false);
            yield break;
        }

        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForSeconds(0.1f);
            if (_inited)
            {
                break;
            }
        }

        if (!_inited)
        {
            Debug.LogWarning("Hyper Web SDK Not Inited.");
            OnInitPhaseFinished?.Invoke(false);
        }
        else
        {
            OnInitPhaseFinished?.Invoke(true);
        }

        // SceneManager.LoadScene(1); // İstersen burayı aç
    }

    public void ShowInterstitialAd()
    {
        if (!_inited)
        {
            Debug.LogWarning("Cannot show ad because Hyper Web SDK not inited.");
            return;
        }

        try
        {
            ShowAd();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("ShowAd call failed: " + ex.Message);
        }
    }

    public void OnInited()
    {
        _inited = true;
        Debug.Log("Hyper Web SDK Inited.");
    }
}
