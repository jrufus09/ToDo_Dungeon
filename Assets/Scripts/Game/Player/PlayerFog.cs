using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PlayerFog : MonoBehaviour {

    [Header("you should set these")] // ignores this header idk why
    int visibleRadius = 2;
    public TileBase fogTile;
    public TileBase seenTile; 

    [Header("code sets these")]
    public DungeonGenerator dungen;
    private int width = 50;
    private int height = 50;
    public Tilemap fogTilemap;
    public enum FogState {
        Dark,
        Seen
    }
    public FogState[,] fogStates;

    void Start() {
        // player is instanced by dungeon gen so it should exist by now. find it:
        if (dungen == null) {
            dungen = FindAnyObjectByType<DungeonGenerator>();
        } // update width/height
        width = dungen.width;
        height = dungen.height;

        // Find tilemap
        var found = GameObject.FindGameObjectsWithTag("FogTilemap");
        if (found.Length != 1) {
            Debug.Log("objects tagged with FogTilemap: "+ found.Length);
        } else {
            fogTilemap = found[0].GetComponent<Tilemap>();
        }

        // new blank canvas
        fogStates = new FogState[width, height];
        FillFog();
    }

    public void FillFog() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                fogStates[i, j] = FogState.Dark;
                fogTilemap.SetTile(new Vector3Int(i, j, 0), fogTile);
            }
        }
    }

    void Update() {
        Vector3 pos = transform.position;
        int x = (int)Mathf.Round(pos.x); // this feels illegal but it's what my sixth form teacher would want
        int y = (int)Mathf.Round(pos.y);
        RevealAt(x, y, visibleRadius);
    }

    public void RevealAt(int x, int y, int radius) {
        for (int dx = -radius; dx <= radius; dx++) {
            for (int dy = -radius; dy <= radius; dy++) {
                int tx = x + dx;
                int ty = y + dy;

                if (InBounds(tx, ty)) {
                    fogStates[tx, ty] = FogState.Seen;
                    fogTilemap.SetTile(new Vector3Int(tx, ty, 0), seenTile);
                }
            }
        }

        // set other visible tiles to seen
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (fogStates[i, j] == FogState.Seen && !IsInRadius(x, y, i, j, radius)) {
                    fogStates[i, j] = FogState.Seen;
                    fogTilemap.SetTile(new Vector3Int(i, j, 0), seenTile);
                }
            }
        }
    }

    private bool InBounds(int x, int y) {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    private bool IsInRadius(int cx, int cy, int x, int y, int radius) {
        return Mathf.Abs(x - cx) <= radius && Mathf.Abs(y - cy) <= radius;
    }
}
