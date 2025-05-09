using UnityEngine;
using System.Collections; // for enumerators
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class SeedGenerator : MonoBehaviour {
    public static SeedGenerator Instance { get; private set; }

    // set random generator seeds for the dungeon generation and enemy generation
    // (only influencing the Unity Random generator)

    public event System.Action OnSeedGenComplete;
    public bool seedGenDone = false;
    public int globalSeed = 0;
    public int width = 40;
    public int height = 40;
    public int roomCount = 8;
    public Vector2Int roomMinSize = new Vector2Int(3, 3);
    public Vector2Int roomMaxSize = new Vector2Int(6, 6);

    public void GenAndSetSeed(Board boardIn) {
        GenerateSeed(boardIn);
        AssignSeed();
        OnSeedGenComplete.Invoke(); // tell dungeon generator to start generating
    }

    void GenerateSeed(Board boardIn) {

        // get name of board
        int bn = HashStringFNV1a(boardIn.name);

        // get number of lists
        List<ListData> lists = BoardDataManager.Instance.AllLists(boardIn);
        int noL = lists.Count;

        // get number of tasks; names of lists
        int noT = 0;
        int hLns = 0; // hash list name sum
        foreach (ListData i in lists) {
            noT = noT + i.items.Count;
            hLns = hLns + HashStringFNV1a(i.name);
        }

        // get number of done tasks
        int noDT = 0;
        foreach (ListData i in lists) {
            noDT = noDT + BoardDataManager.Instance.CountDoneTasks(i);
        }

        // ratio of tasks to done tasks: room size variation
        // the more tasks are done, the more varied the rooms are
        int dtr = noDT/noT*10;
        roomMaxSize = new Vector2Int(roomMinSize.x+dtr, roomMinSize.y+dtr);

        // dungeon gets bigger the more lists and tasks you add
        width = width + noL + noT;
        height = height + noL + noT;

        // seed is sum(name of board, name of each list)
        globalSeed = bn+hLns;
        seedGenDone = true;
    }

    // this was slightly more unique than A1Z26
    int HashStringFNV1a(string input) {
        const uint fnvPrime = 0x01000193; // 16777619
        uint hash = 0x811C9DC5; // 2166136261

        foreach (char c in input) {
            hash ^= c;
            hash *= fnvPrime;
        }

        return unchecked((int)hash); // wrap into a signed int
    }

    public void AssignSeed() {
        if (DungeonGenerator.Instance != null) {
            DungeonGenerator.Instance.seed = globalSeed;
            DungeonGenerator.Instance.width = width;
            DungeonGenerator.Instance.height = height;
            DungeonGenerator.Instance.roomMinSize = roomMinSize;
            DungeonGenerator.Instance.roomMaxSize = roomMaxSize;
        } else {
            Debug.LogWarning("tried to assign seed, but dungeon generator was null");
        }

        if (EnemyHandler.Instance != null) {
            EnemyHandler.Instance.seed = globalSeed;
        } else {
            Debug.LogWarning("tried to assign seed, but enemy handler was null");
        }
    }

}