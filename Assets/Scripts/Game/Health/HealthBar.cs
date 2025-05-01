using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [Header("parent should have Health class")]
    public bool showHealthBar = false;
    public bool hideOnMax = false;

    public Health targetHealth; // parent should have healthbar
    public Image fillImage;
    private Vector3 imgScale;
    
    private GameObject parent;
    public Color fullHealthColour = Color.green;
    public Color lowHealthColour = Color.red;
    
    public Camera canvasCam;
    private Canvas canvas;


    void Start() {
        parent = this.transform.parent.gameObject;
        targetHealth = parent.GetComponent<Health>();

        if (fillImage == null) {
            fillImage = GetComponentInChildren<Image>();
        }
        imgScale = fillImage.transform.localScale;

        canvas = GetComponent<Canvas>();
        if (canvasCam == null) {
            // if not assigned, set to first found by tags
            var found = GameObject.FindGameObjectsWithTag("MainCamera");
            canvasCam = found[0].GetComponent<Camera>();
        }
        canvas.worldCamera = canvasCam;
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

            if (hideOnMax) { // consider listeners for this?
                if (targetHealth.currentHealth == targetHealth.maxHealth) {
                    fillImage.transform.localScale = new Vector3(0, 0, 0); // disappear!
                } else {
                    fillImage.transform.localScale = imgScale;
                }
            }
        }
    }

    void LateUpdate() { // face camera
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;
    }

}
