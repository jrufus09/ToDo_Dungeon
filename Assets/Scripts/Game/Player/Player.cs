using UnityEngine;
using UnityEngine.UI;
using static Cell;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }
    // attach self to the movement buttons because player generates after buttons

    public Vector2Int coordinates;
    public Health health;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep alive between scenes
        } else {
            Destroy(gameObject); // destroy duplicates
        }
    }

    void Start() {
        var pm = GetComponent<PlayerMovement>();
        health = GetComponent<Health>();

        // subscribe to death signal
        health.OnDeath += HandleDeath;

        // get all buttons
        var foundBtns = FindObjectsByType<ButtonMove>(FindObjectsSortMode.None);
        foreach (ButtonMove btn in foundBtns) {
            btn.SetPlayer(pm);
        }

        // initialise camera
        var foundCam = FindObjectsByType<CameraTargetSetter>(FindObjectsSortMode.None);
        var player = GetComponent<Player>();
        foundCam[0].SetPlayer(player);

        // initialise health
        PlayerHealthBar.Instance.SetTargetHealth(GetComponent<Health>());

        EnemyHandler.Instance.SetPlayer(transform);
    }

    void Update() {
        coordinates = Cell.WorldToGrid(transform.position);
    }

    void HandleDeath() {
        PopupManager.Instance.ShowMessagePopup("uh oh, you died!", SceneLoader.Instance.OpenToDo);
    }
}
