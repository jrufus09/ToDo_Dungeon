using UnityEngine;
using Unity.Cinemachine;

public class CameraTargetSetter : MonoBehaviour {
    public CinemachineCamera cam;

    // awake calls before everything else is ready so this must be in start
    
    void Start() {

        if (cam == null) {
            cam = GetComponent<CinemachineCamera>();
        }

        // this wont trigger bc player exists after start
        // GameObject player = GameObject.FindWithTag("Player");

        // if (player != null && cam != null) {
        //     //virtualCam.Follow = player.transform;
        //     cam.Follow = player.transform;
        // } else {
        //     Debug.LogWarning("either player or camera is null");
        // }
    }

    public void SetPlayer(Player playerIn) {
        cam.Follow = playerIn.transform;
    }
}
