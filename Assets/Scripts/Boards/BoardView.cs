using UnityEngine;
using UnityEngine.UI;

public class ViewBoard : MonoBehaviour {

    public Transform contentPane;
    void Start() {

        if (contentPane == null) { // if not set in inspector, get whatever game object has a content size fitter
            GameObject content = GetComponentInChildren<ContentSizeFitter>().gameObject;
            contentPane = content.transform;
        }

        BoardDataManager.Instance.LoadAllListIcons(contentPane);
    }
}
