using UnityEngine;
using UnityEngine.SceneManagement;

public class TempCameraManager : MonoBehaviour
{
    private static GameObject tempCamObj;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void CreateTempCamera()
    {
        if (Camera.main == null && tempCamObj == null)
        {
            tempCamObj = new GameObject("TempCamera");
            Camera cam = tempCamObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.tag = "MainCamera";
            tempCamObj.AddComponent<AudioListener>();

            DontDestroyOnLoad(tempCamObj);
            Debug.Log("[TempCameraManager] Temp camera created.");
        }

        // Listen for scene load to clean up
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Camera main = Camera.main;
        if (main != null && tempCamObj != null)
        {
            Debug.Log("[TempCameraManager] Main camera found, destroying temp camera.");
            Destroy(tempCamObj);
            tempCamObj = null;
        }
    }
}
