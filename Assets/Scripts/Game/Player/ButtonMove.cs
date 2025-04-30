using UnityEngine;
using UnityEngine.UI;

public class ButtonMove : MonoBehaviour {

    public Vector2 inputParameters;
    // UP (0, 1)
    // DOWN (0, -1)
    // LEFT (-1, 0)
    // RIGHT (1, 0)

    public PlayerMovement playerMovement;

    public void SetPlayer(PlayerMovement pIn) {
        playerMovement = pIn;
    }

    void Start() {
        GetComponent<Button>().onClick.AddListener(OnClick);

        if (playerMovement == null) {
            playerMovement = FindAnyObjectByType<PlayerMovement>();
        }
    }

    public void OnClick() {
        playerMovement.Move(inputParameters);
    }
}
