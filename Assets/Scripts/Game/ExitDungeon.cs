using UnityEngine;

public class ExitDungeon : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.layer == 6) { // on player level
            Debug.Log("exit found, returning to main menu");
            SceneLoader.Instance.OpenToDo();
        }
    }
}
