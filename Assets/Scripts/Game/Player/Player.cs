using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    // attach self to the movement buttons because player generates after buttons

    void Start() {
        var pm = GetComponent<PlayerMovement>();

        // get all buttons
        var foundBtns = FindObjectsByType<ButtonMove>(FindObjectsSortMode.None);
        foreach (ButtonMove btn in foundBtns) {
            btn.SetPlayer(pm);
        }

        // initialise camera
        var foundCam = FindObjectsByType<CameraTargetSetter>(FindObjectsSortMode.None);
        var player = GetComponent<Player>();
        foundCam[0].SetPlayer(player);

    }
}
