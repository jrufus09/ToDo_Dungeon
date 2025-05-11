using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : HealthBar {

    // key difference is that this canvas is part of the HUD
    public static PlayerHealthBar Instance { get; private set; }
    Transform player;
    
    void Start() {

        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep alive between scenes
        } else {
            Destroy(gameObject); // destroy duplicates
        }

        
        if (fillImage == null) {
            fillImage = GetComponentInChildren<Image>();
        }

        // player = Player.Instance.transform;
        // targetHealth = Player.Instance.gameObject.GetComponent<Health>();

    }

    // // returns self as an object if needed, takes object in
    public Transform SetTargetHealth(Health healthComponent) {
        targetHealth = healthComponent;
        return this.transform;
    }

    void Update() {
        if (showHealthBar) { // if we actually show the health bar
            if (targetHealth != null && fillImage != null) {
                float h = (float)targetHealth.currentHealth / targetHealth.maxHealth;
                // h is the % of health left

                // update colour
                if (h <= 0.4f) {
                    fillImage.color = Color.Lerp(lowHealthColour, fullHealthColour, h);
                }

                // bar reduce
                fillImage.fillAmount = h;
            }

        }
    }

}
