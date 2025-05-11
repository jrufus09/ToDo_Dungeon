using UnityEngine;
using Unity.Cinemachine;

public class CameraTargetSetter : MonoBehaviour {
    public CinemachineCamera cam;

    // awake calls before everything else is ready so this must be in start
    
    void Start() {

        if (cam == null) {
            cam = GetComponent<CinemachineCamera>();
        }
    }

    public void SetPlayer(Player playerIn) {
        cam.Follow = playerIn.transform;
    }
}
