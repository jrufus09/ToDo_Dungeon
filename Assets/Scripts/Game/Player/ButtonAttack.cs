using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAttack : MonoBehaviour {
    //

    public List<Vector2> directions;
    // ideally later one would sort this into a dropdown and use that to filter what we attack

    void Start() {
        directions = new List<Vector2>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {

        // directions is set by dungeonui which controls this button's interactability
        // instead of sorting into dropdown and having a selection from there, im just gonna get first dir

        // convert from vector2.left, etc. to Vector2Int

        PlayerMovement.Instance.PlayerAttack(DirtoInt(directions[0]));
    }

    public Vector2Int DirtoInt(Vector2 dir) {
        Vector2Int dirOut = new Vector2Int();

        if (dir==Vector2.down) {
            dirOut = new Vector2Int(0, -1);
        } else if (dir==Vector2.up) {
            dirOut = new Vector2Int(0, 1);
        } else if (dir==Vector2.left) {
            dirOut = new Vector2Int(-1, 0);
        } else if (dir==Vector2.right) {
            dirOut = new Vector2Int(1, 0);
        }

        return dirOut;
    }

}
