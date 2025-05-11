using UnityEngine;

public class CameraDebugger : MonoBehaviour
{
    void LateUpdate()
    {
        Camera mainCam = Camera.main;
        var allCams = Camera.allCameras;

        string log = $"[CameraDebugger] Frame {Time.frameCount} — ";
        log += mainCam ? $"MainCamera: {mainCam.name}" : "MainCamera: null";
        log += $" | Total Cameras: {allCams.Length} [";

        for (int i = 0; i < allCams.Length; i++)
        {
            log += $"{allCams[i].name} (enabled: {allCams[i].enabled})";
            if (i < allCams.Length - 1) log += ", ";
        }

        log += "]";

        Debug.Log(log);
    }
}
