using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPrefab; // Assign in Inspector (optional)
    
    public void ShowInputPopup(string title, System.Action<string> onConfirm)
    {
        // find canvas
        GameObject canvas = Object.FindFirstObjectByType<Canvas>()?.gameObject;

        // background panel
        GameObject panel = new GameObject("PopupPanel", typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 200);
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f); // Dark background

        GameObject titleText = new GameObject("Title", typeof(Text));
        titleText.transform.SetParent(panel.transform, false);
        Text titleComponent = titleText.GetComponent<Text>();
        titleComponent.text = title;
        titleComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleComponent.alignment = TextAnchor.MiddleCenter;
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(380, 40);
        titleRect.anchoredPosition = new Vector2(0, 60);

        // input field (type here)
        GameObject inputFieldObj = new GameObject("InputField", typeof(Image));
        inputFieldObj.transform.SetParent(panel.transform, false);
        InputField inputField = inputFieldObj.AddComponent<InputField>();
        inputField.textComponent = inputFieldObj.AddComponent<Text>();
        inputField.textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        inputField.textComponent.alignment = TextAnchor.MiddleLeft;
        inputField.placeholder = inputFieldObj.AddComponent<Text>();
        //inputField.placeholder.text = "Enter text...";
        //inputField.placeholder.font = inputField.textComponent.font;
        RectTransform inputRect = inputFieldObj.GetComponent<RectTransform>();
        inputRect.sizeDelta = new Vector2(350, 40);
        inputRect.anchoredPosition = new Vector2(0, 10);

        // confirm button
        GameObject confirmBtn = CreateButton(panel.transform, "Confirm", new Vector2(-70, -60), () => 
        {
            onConfirm?.Invoke(inputField.text);
            Destroy(panel);
        });

        // Create Cancel Button
        GameObject cancelBtn = CreateButton(panel.transform, "Cancel", new Vector2(70, -60), () => 
        {
            Destroy(panel);
        });
    }

    private GameObject CreateButton(Transform parent, string text, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObj = new GameObject(text + "Button", typeof(Image), typeof(Button));
        buttonObj.transform.SetParent(parent, false);
        buttonObj.GetComponent<Image>().color = Color.gray;
        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 40);
        rect.anchoredPosition = position;

        GameObject buttonText = new GameObject("Text", typeof(Text));
        buttonText.transform.SetParent(buttonObj.transform, false);
        Text txt = buttonText.GetComponent<Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.rectTransform.sizeDelta = new Vector2(100, 40);

        buttonObj.GetComponent<Button>().onClick.AddListener(onClick);
        return buttonObj;
    }
}
