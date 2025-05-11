using UnityEngine;

public class ExitDungeon : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.layer == 6) { // on player level
            //Debug.Log("exit found, returning to main menu");
            PopupManager.Instance.ShowMessagePopup("You found your way out of the dungeon!", SceneLoader.Instance.OpenToDo);
        }
    }
}
